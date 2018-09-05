using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using log4net;
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
        }

        public struct MappingsResponse
        {
            public Mapping entry;
            public MappingLine[] lines;
        }

        private readonly ModelContext modelContext;

        public ImportController(ModelContext modelContext)
        {
            this.modelContext = modelContext;
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
            if (file == null || file.Length == 0)
            {
                return JsonConvert.SerializeObject(new ApiResponse { status = "fail",
                    message = "no file provided" });
            }

            var optionsBuilder = new DbContextOptionsBuilder<ModelContext>();
            var dbSerializer = new Core.Import.DbSerializer(modelContext);
            ILog logger = log4net.LogManager.GetLogger(
                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            var errorLogger = new Core.Import.ErrorLogger(logger);
            var service = new Core.Import.Service(dbSerializer, errorLogger);

            var filePath = Path.GetTempFileName();

            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            var mapping = new Mapping() { DataLineNumber = startLine };
            var mappingLines = new List<MappingLine>();
            var mappingTableJson = JsonConvert.DeserializeObject<List<MappingLine>>(mappingTable);
            var protocolSetJson = JsonConvert.DeserializeObject<ProtocolSet>(protocolSet);

            try {
                service.Execute(filePath, protocolSetJson, mapping, mappingTableJson);
            } catch(Core.Import.ImportException err) {
                return JsonConvert.SerializeObject(
                    new ApiResponse { status = "fail", message = err.ToString() });
            } finally {
                System.IO.File.Delete(filePath);
            }

            return JsonConvert.SerializeObject(new ApiResponse { status = "ok" });
        }

        [HttpGet, Route("api/import/protocolSets/{id}")]
        public ProtocolSet ProtocolSet(int id)
        {
            return modelContext.Find<ProtocolSet>(id);
        }

        [HttpPatch, Route("api/import/protocolSets/{id}")]
        public ApiResponse Update(int id, string protocolSet)
        {
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
            var result = new List<MappingsResponse>();
            var mappings = modelContext.Set<Mapping>();
            foreach (Mapping mapping in mappings)
            {
                var element = new MappingsResponse()
                {
                    entry = mapping,
                    lines = modelContext.Set<MappingLine>()
                        .Where(m => m.MappingId == mapping.Id).ToArray()
                };
                result.Add(element);
            }

            return result;
        }

        [HttpPost, Route("api/import/mappings")]
        public ApiResponse CreateMapping(string name, string mappingTable)
        {
            var mapping = new Mapping() { Name = name };
            modelContext.Add(mapping);
            modelContext.SaveChanges();

            var mappingTableJson = JsonConvert.DeserializeObject<List<MappingLine>>(mappingTable);
            foreach (MappingLine line in mappingTableJson)
            {
                line.MappingId = mapping.Id;
                modelContext.Add(line);
            }

            modelContext.SaveChanges();

            return new ApiResponse { status = "ok" };
        }
    }
}
