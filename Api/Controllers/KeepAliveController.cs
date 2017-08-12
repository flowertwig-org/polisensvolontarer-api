using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class KeepAliveController : Controller
    {
        // GET api/values
        [HttpGet]
        public bool Get()
        {
            // TODO: 1. Sanity checking
            // TODO: 2. Validate current session
            // TODO: 3. If valid, update session (TO keep alive)
            // TODO: 4. Return current state
            return false;
        }
    }
}