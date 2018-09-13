using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using ElectionStatistics.Core;
using ElectionStatistics.Model;

namespace ElectionStatistics.WebSite
{
    // TODO: tests
    public class ImportController : Controller
    {
        public struct ApiResponse
        {
            public string status;
            public string message;
            public object data;
        }

        public struct MappingsResponse
        {
            public Mapping entry;
            public MappingLine[] lines;
        }

        public struct ImportProgressData
        {
            public int progress;
            public bool? success;
        }

        private readonly ModelContext modelContext;
        private readonly Core.Import.IBackgroundQueue importQueue;

        public ImportController(ModelContext modelContext,
            Core.Import.IBackgroundQueue importQueue)
        {
            this.modelContext = modelContext;
            this.importQueue = importQueue;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("admin") != null)
            {
                return View();
            }
            else
            {
                return Redirect("/signIn");
            }
        }

        [HttpPost, Route("api/import/protocolSets")]
        public string Create(IFormFile file, string mappingTable, string protocolSet,
                             int startLine)
        {
            if (HttpContext.Session.GetString("admin") == null)
            {
                return null;
            }

            if (file == null || file.Length == 0)
            {
                return JsonConvert.SerializeObject(new ApiResponse
                {
                    status = "fail", message = "no file provided"
                });
            }

            var protocolSetJson = JsonConvert.DeserializeObject<ProtocolSet>(protocolSet);
            if (protocolSetJson.TitleRus.Length == 0) {
                return JsonConvert.SerializeObject(new ApiResponse
                {
                    status = "fail", message = "empty 'TitleRus' field"
                });
            }

            if (modelContext.Set<ProtocolSet>().Any(ps => ps.TitleRus == protocolSetJson.TitleRus))
            {
                return JsonConvert.SerializeObject(new ApiResponse
                {
                    status = "fail",
                    message = $"ProtocolSet '{protocolSetJson.TitleRus}' already exists"
                });
            }

            var dbSerializer = new Core.Import.DbSerializer(modelContext);
            var errorLogger = new Core.Import.ErrorLogger();

            var service = new Core.Import.Service(dbSerializer, errorLogger);

            var filePath = Path.GetTempFileName();

            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            var mapping = new Mapping() { DataLineNumber = startLine };
            var mappingLines = new List<MappingLine>();
            var mappingTableJson = JsonConvert.DeserializeObject<List<MappingLine>>(mappingTable);

            try {
                service.Configure(filePath, protocolSetJson, mapping,
                    mappingTableJson).Initialize();

                importQueue.Enqueue(async (token, dbc) => {
                    try
                    {
                        await Task.Run(() => {
                            // Reinitialize db serializer due to
                            // System.ObjectDisposedException:
                            // Cannot access a disposed object.
                            var serializer = new Core.Import.DbSerializer(dbc);
                            var notifier =
                                new Core.Import.DbProgressNotifier(serializer,
                                    protocolSetJson);

                            service.Execute(serializer, notifier);
                        });
                    }
                    finally
                    {
                        // TODO: DRY
                        System.IO.File.Delete(filePath);
                    }
                });
            } catch(Core.Import.ImportException err) {
                // TODO: DRY
                System.IO.File.Delete(filePath);

                return JsonConvert.SerializeObject(
                    new ApiResponse { status = "fail", message = err.ToString() });
            }

            return JsonConvert.SerializeObject(
                new ApiResponse { status = "ok", data = protocolSetJson.Id });
        }

        [HttpGet, Route("api/import/protocolSets")]
        public IEnumerable<ProtocolSet> ProtocolSets()
        {
            if (HttpContext.Session.GetString("admin") == null)
            {
                return null;
            }

            return modelContext.Set<ProtocolSet>().AsNoTracking()
                .OrderBy(protocolSet => protocolSet.TitleRus);
        }

        [HttpGet, Route("api/import/protocolSets/{id}")]
        public ProtocolSet ProtocolSet(int id)
        {
            if (HttpContext.Session.GetString("admin") == null)
            {
                return null;
            }

            return modelContext.Find<ProtocolSet>(id);
        }

        [HttpGet, Route("api/import/protocolSets/{id}/log")]
        public IActionResult ImportErrorLog(int id)
        {
            if (HttpContext.Session.GetString("admin") == null)
            {
                return null;
            }

            var protocol = modelContext.Find<ProtocolSet>(id);
            if (protocol == null || !System.IO.File.Exists(protocol.ImportFileErrorLog))
            {
                return NotFound();
            }

            var file = System.IO.File.ReadAllBytes(protocol.ImportFileErrorLog);
            return File(file, "text/plain", $"import-protocolSet-{id}.log");
        }

        [HttpGet, Route("/api/import/protocolSets/{id}/progress")]
        public ApiResponse ImportProgress(int id)
        {
            var protocolSet = modelContext.Find<ProtocolSet>(id);
            if (protocolSet != null)
            {
                var success = protocolSet.ImportSuccess;
                var data = new ImportProgressData();
                if (protocolSet.ImportFinishedAt != null)
                {
                    data.success = protocolSet.ImportSuccess;
                }
                data.progress = ImportProgressRate(protocolSet);
                return new ApiResponse() { status = "ok", data = data };
            }
            else
            {
                return new ApiResponse()
                {
                    status = "fail", message = $"ProtocolSet ID {id} not found"
                };
            }
        }

        [HttpPatch, Route("api/import/protocolSets/{id}")]
        public ApiResponse Update(int id, string protocolSet)
        {
            if (HttpContext.Session.GetString("admin") == null)
            {
                return new ApiResponse { status = "fail", message = "Not found"};
            }

            var protocolSetJson = JsonConvert.DeserializeObject<ProtocolSet>(protocolSet);
            if (id != protocolSetJson.Id)
            {
                return new ApiResponse { status = "fail", message = "Not found"};
            }

            modelContext.Update(protocolSetJson);
            modelContext.SaveChanges();
            return new ApiResponse { status = "ok" };
        }

        [HttpGet, Route("api/import/mappings")]
        public List<MappingsResponse> Mappings()
        {
            if (HttpContext.Session.GetString("admin") == null)
            {
                return null;
            }

            var result = new List<MappingsResponse>();
            var mappings = modelContext.Set<Mapping>().OrderBy(mapping => mapping.Id);
            foreach (Mapping mapping in mappings)
            {
                var element = new MappingsResponse()
                {
                    entry = mapping,
                    lines = modelContext.Set<MappingLine>()
                        .Where(ml => ml.MappingId == mapping.Id)
                        .OrderBy(ml => ml.ColumnNumber).ToArray()
                };
                result.Add(element);
            }

            return result;
        }

        [HttpPost, Route("api/import/mappings")]
        public ApiResponse CreateMapping(string name, int dataLineNumber, string mappingTable)
        {
            if (HttpContext.Session.GetString("admin") == null)
            {
                return new ApiResponse { status = "fail" };
            }

            var mapping = new Mapping() { Name = name, DataLineNumber = dataLineNumber };
            modelContext.Add(mapping);
            modelContext.SaveChanges();

            var mappingTableJson = JsonConvert.DeserializeObject<List<MappingLine>>(mappingTable);
            foreach (MappingLine line in mappingTableJson)
            {
                line.Id = 0;
                line.MappingId = mapping.Id;
                modelContext.Add(line);
            }

            modelContext.SaveChanges();

            return new ApiResponse { status = "ok" };
        }

        private int ImportProgressRate(ProtocolSet protocolSet)
        {
            var rate = ((double)protocolSet.ImportCurrentLine / protocolSet.ImportTotalLines);
            return double.IsNaN(rate) ? 0 : (int)(rate * 100);
        }
    }
}
