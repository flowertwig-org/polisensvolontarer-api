using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        // GET api/values
        [HttpGet]
        public bool Get()
        {
            // TODO: 1. Sanity checking
            // TODO: 2. Return login status
            return false;
        }

        // POST api/values
        [HttpPost]
        public bool Post([FromBody]string username, [FromBody]string password)
        {
            // TODO: 1. Sanity checking
            // TODO: 2. Make login request
            // TODO: 3. If no valid login, return false
            // TODO: 4a. Add session info
            // TODO: 4b. return true;
            return false;
        }
    }
}