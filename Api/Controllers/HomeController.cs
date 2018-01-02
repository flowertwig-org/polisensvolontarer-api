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
            this.Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, post-check=0, pre-check=0");

            return this.Redirect("https://polisensvolontarer.azurewebsites.net/login/");
        }
    }
}
