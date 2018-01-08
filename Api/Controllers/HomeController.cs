using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        private AppSettings _appSettings;
        public HomeController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            this.Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, post-check=0, pre-check=0");

            return this.Redirect(_appSettings.WebSiteUrl + "/login/");
        }
    }
}
