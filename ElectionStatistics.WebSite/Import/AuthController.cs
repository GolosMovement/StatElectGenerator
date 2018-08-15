using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace ElectionStatistics.WebSite {

    [Route("api")]
    public class AuthController : Controller
    {
        public struct Settings
        {
            public struct Admin
            {
                public string user;
                public string password;
            }

            public Admin admin;
        }

        // TODO: migrate to normal db auth
        [HttpPost, Route("signIn")]
        public bool SignIn([FromBody] Settings.Admin admin)
        {
            String json = System.IO.File.ReadAllText("appsettings.json");
            Settings credentials = JsonConvert.DeserializeObject<Settings>(json);

            var result = credentials.admin.user == admin.user &&
                credentials.admin.password == admin.password;
            if (result) HttpContext.Session.SetString("admin", "true");

            return result;
        }
    }
}
