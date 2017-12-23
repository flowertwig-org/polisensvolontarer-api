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
            HttpContext.Session.Clear();
            return Json(true);
        }

        [HttpPost]
        public IActionResult Post()
        {
            HttpContext.Session.Clear();
            return Json(true);
        }
    }
}