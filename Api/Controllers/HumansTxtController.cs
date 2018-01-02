using System;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("/humans.txt")]
    public class HumansTxtController : Controller
    {
        [HttpGet]
        public IActionResult Get() {
            this.Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, post-check=0, pre-check=0");

            return this.Content("/*Developer(s)*/\r\nMattias");
        }
    }
}
