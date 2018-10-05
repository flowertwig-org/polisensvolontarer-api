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
        public AvailableAssignmentsResult Get(string key, int startIndex = 0, int nOfItems = -1,
            string filterAlwaysShowTypes = null, string filterNeverShowTypes = null,
            string filterHideWorkDayTypes = null, string filterHideWeekendTypes = null,
            string filterNeverShowAreas = null, string filterAlwaysShowAreas = null,
            string filterNeverShowSpecTypes = null)
        {
            this.Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, post-check=0, pre-check=0");

            var totalNofItems = 0;
            // TODO: 1. Sanity checking
            // TODO: 2. Validate login
            List<Assignment> list = AvailableAssignmentsHelper.GetAvailableAssignments(this.HttpContext);
            totalNofItems = list.Count;

            // filter items
            var filterNofItems = 0;
            var filterSettings = AvailableAssignmentsHelper.GetFilterSettings(filterAlwaysShowTypes,
                filterNeverShowTypes,
                filterHideWorkDayTypes,
                filterHideWeekendTypes,
                filterNeverShowAreas,
                filterAlwaysShowAreas,
                filterNeverShowSpecTypes
            );

            list = AvailableAssignmentsHelper.FilerItems(list, filterSettings);

            // only get ONE item, if available
            if (key != null)
            {
                list = list.Where(a => a.Id == key).ToList();
            }

            // support paging, start position
            if (startIndex > 0)
            {
                list = list.Skip(startIndex).ToList();
            }

            // support paging, number of items
            if (nOfItems > 0)
            {
                list = list.Take(nOfItems).ToList();
            }

            // 3. Return available assignmets
            var groupedList = list.GroupBy(AvailableAssignmentsHelper.GroupByDay);

            return new AvailableAssignmentsResult {
                TotalNumberOfItems = totalNofItems,
                FilteredNofItems = filterNofItems,
                Items = groupedList.ToArray()
            };
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