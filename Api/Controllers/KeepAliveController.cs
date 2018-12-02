using System.Collections.Generic;
using Api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class KeepAliveController : Controller
    {
        // GET api/values
        [HttpGet]
        public int Get()
        {
            this.Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, post-check=0, pre-check=0");

            // TODO: 1. Sanity checking
            // TODO: 2. Validate current session
            try
            {
                byte[] data;
                if (HttpContext.Session.TryGetValue("Session-Cookie", out data))
                {
                    string content = System.Text.Encoding.UTF8.GetString(data);
                    if (string.IsNullOrEmpty(content))
                    {
                        return -1;
                    }
                }
                else
                {
                    return 0;
                }
                // TODO: 1. Sanity checking
                // TODO: 2. Return login status
                // TODO: 3. If no valid login, return false
                // TODO: 4a. Add session info
                // TODO: 4b. return true;
            }
            catch (System.Exception)
            {
                return -2;
            }

            // TODO: 3. If valid, update session (TO keep alive)
            // TODO: 4. Return current state
            // TODO: 1. Sanity checking
            // TODO: 2. Validate login
            var list = new List<Assignment>();
            try
            {
                byte[] data;
                if (HttpContext.Session.TryGetValue("AvailableAssignments", out data))
                {
                    var content = System.Text.Encoding.UTF8.GetString(data);
                    var jArray = Newtonsoft.Json.JsonConvert.DeserializeObject(content) as Newtonsoft.Json.Linq.JArray;
                    var array = jArray.ToObject<Assignment[]>();
                    if (array != null)
                    {
                        list.AddRange(array);
                    }
                }
            }
            catch (System.Exception)
            {
                // TODO: Do error handling
                return -3;
            }

            // TODO: 3. Return available assignmets
            if (list.Count == 0)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
    }
}