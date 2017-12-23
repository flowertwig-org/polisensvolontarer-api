using System;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("/humans.txt")]
    public class HumansTxtController : Controller
    {
        [HttpGet]
        public IActionResult Get() {
            return this.Content("/*Developer(s)*/\r\nMattias");
        }
    }
}
