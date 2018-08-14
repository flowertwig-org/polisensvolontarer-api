using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Api.Contracts;
using System.Linq;
using Api.Helpers;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class AvailableAssignmentsController : Controller
    {
        private AppSettings _appSettings;
        public AvailableAssignmentsController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        // GET api/Assignments/{id}
        [HttpGet]
        //[ResponseCache(VaryByHeader = "Cookie", VaryByQueryKeys = new[] { "key" }, Duration = 60)]
        public IGrouping<string, Assignment>[] Get(string key)
        {
            this.Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, post-check=0, pre-check=0");

            // TODO: 1. Sanity checking
            // TODO: 2. Validate login
            List<Assignment> list = AvailableAssignmentsHelper.GetAvailableAssignments(this.HttpContext);
            if (key != null)
            {
                list = list.Where(a => a.Id == key).ToList();
            }
            // TODO: 3. Return available assignmets
            var groupedList = list.GroupBy(AvailableAssignmentsHelper.GroupByDay);
            return groupedList.ToArray();
        }


        // POST api/AvailableAssignments
        [HttpPost]
        public ActionResult Post([FromForm]string key,[FromForm] string comment,[FromForm] string password)
        {
            List<Assignment> list = AvailableAssignmentsHelper.GetAvailableAssignments(this.HttpContext);
            var item = list.FirstOrDefault(a => a.Id == key);
            if (item == null)
            {
                return NotFound();
            }

            // TODO: 1. Sanity checking
            // TODO: 2. Validate login
            // TODO: 3. Sign up for specific assignmet

            var cookieContainer = new CookieContainer();

            byte[] cookieData;
            if (HttpContext.Session.TryGetValue("Session-Cookie", out cookieData))
            {
                var sessionCookie = System.Text.Encoding.UTF8.GetString(cookieData);
                cookieContainer.Add(new System.Uri("http://volontar.polisen.se/"), new Cookie("PHPSESSID", sessionCookie));

                using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                {
                    var assignment = AssignmentDetailHelper.GetAssignmentDetailFromUrl(handler, item, _appSettings.WebSiteUrl);

                    if (assignment == null)
                    {
                        return NotFound();
                    }


                    AssignmentDetailHelper.SubmitInterestOfAssignment(handler, assignment, comment, password);

                    return this.Ok();
                }
            }

            return this.Unauthorized();
            //return this.Redirect(_appSettings.WebSiteUrl + "/login?page=assignment&key=" + key);
        }
    }
}