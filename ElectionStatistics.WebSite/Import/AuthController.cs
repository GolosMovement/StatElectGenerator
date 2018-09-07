using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace ElectionStatistics.WebSite {

    public class AuthController : Controller
    {
        private AppSettings AppSettings;

        public AuthController(IOptions<AppSettings> settings)
        {
            AppSettings = settings.Value;
        }

        public struct Settings
        {
            public struct Admin
            {
                public string user;
                public string password;
            }

            public Admin admin;
        }

        [HttpGet, Route("admin")]
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

        // TODO: migrate to normal db auth
        [HttpPost, Route("api/signIn")]
        public bool SignIn([FromBody] Settings.Admin admin)
        {
            var result = admin.user == "admin" && admin.password == AppSettings.AdminPassword;
            if (result) HttpContext.Session.SetString("admin", "true");

            return result;
        }
    }
}
