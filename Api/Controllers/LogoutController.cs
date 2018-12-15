using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class LogoutController : Controller
    {
        private AppSettings _appSettings;
        public LogoutController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        [HttpPost]
        public IActionResult Post()
        {
            this.Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, post-check=0, pre-check=0");

            HttpContext.Session.Clear();

            return this.Redirect(_appSettings.WebSiteUrl + "/?logout=1");
        }
    }
}