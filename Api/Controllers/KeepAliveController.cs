using System.Collections.Generic;
using Api.Contracts;
using Api.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class KeepAliveController : Controller
    {
        // GET api/values
        [HttpGet]
        public int Get(string cookieFailKey = null)
        {
            this.Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, post-check=0, pre-check=0");

            try
            {
                var keyInfo = new CookieFailKeyInfo(cookieFailKey);

                var list = AvailableAssignmentsHelper.GetAvailableAssignments(HttpContext, keyInfo);
                if (list.Count == 0)
                {
                    return 1;
                }
                else
                {
                    return 2;
                }
            }
            catch (System.Exception)
            {
                return -2;
            }
        }
    }
}