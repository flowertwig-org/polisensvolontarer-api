using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Api.Contracts;
using Api.Helpers;
using System.Net.Http;
using System.Net;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class MyAssignmentsController : Controller
    {
        // GET api/values
        [HttpGet]
        public MyAssignmentsInfo Get(string cookieFailKey = null)
        {
            this.Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, post-check=0, pre-check=0");

            var keyInfo = new CookieFailKeyInfo(cookieFailKey);

            try
            {
                return MyAssignmentsHelper.GetMyAssignments(this.HttpContext, keyInfo);
            }
            catch (System.Exception ex)
            {
                // TODO: We don't know if we are logged in BUT while debugging we need to be able to check this
                return new MyAssignmentsInfo { IsLoggedIn = true, Reservations = new List<Assignment> { new AssignmentDetail { Description = "Unknown Error2: " + ex } } };
            }
        }
    }
}