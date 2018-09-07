using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

            try
            {
                var validator = new Core.Preset.Validator(modelContext, new Core.Preset.Parser());
                validator.Execute(presetJson.Expression,
                    modelContext.Find<ProtocolSet>(presetJson.ProtocolSetId));
            }
            catch (Core.Preset.ValidationException ex)
            {
                return new ImportController.ApiResponse() { status = "fail", message = ex.Message };
            }

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

            try
            {
                var validator = new Core.Preset.Validator(modelContext, new Core.Preset.Parser());
                validator.Execute(presetJson.Expression,
                    modelContext.Find<ProtocolSet>(presetJson.ProtocolSetId));
            }
            catch (Core.Preset.ValidationException ex)
            {
                return new ImportController.ApiResponse() { status = "fail", message = ex.Message };
            }

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
    }
}
