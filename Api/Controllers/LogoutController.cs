using System;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class LogoutController : Controller
    {
        // TODO: We should change this to only use POST
        [HttpGet]
        public IActionResult Get()
        {
            this.Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, post-check=0, pre-check=0");

            HttpContext.Session.Clear();
            return Json(true);
        }

        [HttpPost]
        public IActionResult Post()
        {
            this.Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, post-check=0, pre-check=0");

            HttpContext.Session.Clear();
            return Json(true);
        }
    }
}