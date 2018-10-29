using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EFCore.BulkExtensions;

using Newtonsoft.Json;

using ElectionStatistics.Model;

namespace ElectionStatistics.WebSite
{
    public class PresetManagerController : Controller
    {
        private readonly ModelContext modelContext;

        public PresetManagerController(ModelContext modelContext)
        {
            this.modelContext = modelContext;
        }

        [HttpGet, Route("presets")]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("admin") != null)
            {
                return View();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet, Route("api/presets/{id}")]
        public Preset Preset(int id)
        {
            if (HttpContext.Session.GetString("admin") == null)
            {
                return null;
            }

            return modelContext.Find<Preset>(id);
        }

        // TODO: move ImportController.ApiResponse to shared module
        [HttpPost, Route("/api/presets/")]
        public ImportController.ApiResponse CreatePreset(string preset)
        {
            if (HttpContext.Session.GetString("admin") == null)
            {
                return null;
            }

            var presetJson = JsonConvert.DeserializeObject<Preset>(preset);
            var protocolSet = modelContext.Find<ProtocolSet>(presetJson.ProtocolSetId);
            try
            {
                var validator = new Core.Preset.Validator(modelContext, new Core.Preset.Parser());
                validator.Execute(presetJson.Expression, protocolSet);
            }
            catch (Core.Preset.ValidationException ex)
            {
                return new ImportController.ApiResponse() { status = "fail", message = ex.Message };
            }

            protocolSet.ShouldRecalculatePresets = true;
            modelContext.Set<Preset>().Add(presetJson);
            modelContext.SaveChanges();

            return new ImportController.ApiResponse() { status = "ok" };
        }

        [HttpPatch, Route("/api/presets/{id}")]
        public ImportController.ApiResponse UpdatePreset(int id, string preset)
        {
            if (HttpContext.Session.GetString("admin") == null)
            {
                return null;
            }

            var presetJson = JsonConvert.DeserializeObject<Preset>(preset);
            if (id != presetJson.Id)
            {
                return new ImportController.ApiResponse { status = "fail", message = "Not found" };
            }

            var protocolSet = modelContext.Find<ProtocolSet>(presetJson.ProtocolSetId);
            try
            {
                var validator = new Core.Preset.Validator(modelContext, new Core.Preset.Parser());
                validator.Execute(presetJson.Expression, protocolSet);
            }
            catch (Core.Preset.ValidationException ex)
            {
                return new ImportController.ApiResponse() { status = "fail", message = ex.Message };
            }

            protocolSet.ShouldRecalculatePresets = true;
            modelContext.Update(presetJson);
            modelContext.SaveChanges();
            return new ImportController.ApiResponse() { status = "ok" };
        }

        [HttpDelete, Route("/api/presets/{id}")]
        public ImportController.ApiResponse DestroyPreset(int id)
        {
            if (HttpContext.Session.GetString("admin") == null)
            {
                return null;
            }

            modelContext.Remove<Preset>(modelContext.Find<Preset>(id));
            modelContext.SaveChanges();

            return new ImportController.ApiResponse() { status = "ok" };
        }

        [HttpPost, Route("api/presets/recalc/protocolSet/{protocolSetId}")]
        public ImportController.ApiResponse RecalcPresets(int protocolSetId)
        {
            if (HttpContext.Session.GetString("admin") == null)
            {
                return null;
            }

            var protocolSet = modelContext.Find<ProtocolSet>(protocolSetId);
            if (!protocolSet.ShouldRecalculatePresets)
            {
                return null;
            }

            var presets = modelContext.Set<Preset>()
                .Where(preset => preset.ProtocolSetId == protocolSetId).AsNoTracking();
            var lineCalculatedValues = modelContext.Set<LineCalculatedValue>()
                .Where(lcv => presets.Select(preset => preset.Id).Contains(lcv.PresetId))
                .AsNoTracking();

            var parser = new Core.Preset.Parser();

            using (var transaction = modelContext.Database.BeginTransaction())
            {
                var bulkConfig = new BulkConfig { BulkCopyTimeout = 0 };

                modelContext.BulkDelete(lineCalculatedValues.ToList(), bulkConfig);
                presets.ToList().ForEach(preset =>
                {
                    var service = new Core.Preset.Calculator(modelContext, parser, preset);
                    modelContext.BulkInsert(service.Execute(), bulkConfig);
                });
                protocolSet.ShouldRecalculatePresets = false;
                modelContext.BulkUpdate(new List<ProtocolSet>() { protocolSet }, bulkConfig);
                transaction.Commit();
            }

            return new ImportController.ApiResponse() { status = "ok" };
        }
    }
}
