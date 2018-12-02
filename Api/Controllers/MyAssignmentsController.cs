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
        public MyAssignmentsInfo Get()
        {
            this.Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, post-check=0, pre-check=0");

            // TODO: 1. Sanity checking
            // TODO: 2. Validate login
            // TODO: 3. Return my assignmets

            try
            {
                var cookieContainer = new CookieContainer();

                byte[] cookieData;
                if (HttpContext.Session.TryGetValue("Session-Cookie", out cookieData))
                {
                    var sessionCookie = System.Text.Encoding.UTF8.GetString(cookieData);
                    cookieContainer.Add(new System.Uri("http://volontar.polisen.se/"), new Cookie("PHPSESSID", sessionCookie));

                    using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                    {
                        byte[] data;
                        if (this.HttpContext.Session.TryGetValue("MainNavigation", out data))
                        {
                            var content = System.Text.Encoding.UTF8.GetString(data);
                            var jArray = Newtonsoft.Json.JsonConvert.DeserializeObject(content) as Newtonsoft.Json.Linq.JObject;
                            var navigationInfo = jArray.ToObject<NavigationInfo>();
                            if (navigationInfo != null)
                            {
                                return MyAssignmentsHelper.GetMyAssignmentsInfoFromUrl(handler, navigationInfo);
                            }
                        }
                        return new MyAssignmentsInfo { IsLoggedIn = true };
                    }
                }
                return new MyAssignmentsInfo { IsLoggedIn = false };
            }
            catch (System.Exception ex)
            {
                // TODO: We don't know if we are logged in BUT while debugging we need to be able to check this
                return new MyAssignmentsInfo { IsLoggedIn = true, Reservations = new List<Assignment> { new AssignmentDetail { Description = "Unknown Error2: " + ex } } };
            }
        }
    }
}