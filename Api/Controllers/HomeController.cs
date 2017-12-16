using System;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            return this.Redirect("https://polisensvolontarer.azurewebsites.net/login/");
        }
    }
}
