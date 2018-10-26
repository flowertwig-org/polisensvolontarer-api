using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Api.Contracts;
using Api.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class AssignmentReportController : Controller
    {
        private AppSettings _appSettings;
        public AssignmentReportController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        // GET api/Assignment/{id}
        [HttpGet]
        //[ResponseCache(VaryByQueryKeys = new[] { "key" }, Duration = 60)]
        public JsonResult Get(string key)
        {
            try
            {
                this.Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, post-check=0, pre-check=0");

                return Json(null);
            }
            catch (System.Exception ex)
            {
                return Json(new Assignment { Name = ex.Message });
            }
        }

        // POST api/AvailableAssignments
        [HttpPost]
        public bool Post(
            [FromForm]string name, [FromForm]string email,
            [FromForm]string assignmentOrDate, [FromForm]int areaIndex,
            [FromForm]string feedback1, [FromForm]string feedback2, [FromForm]string feedback3)
        {
            var cookieContainer = new CookieContainer();

            byte[] cookieData;
            if (HttpContext.Session.TryGetValue("Session-Cookie", out cookieData))
            {
                var sessionCookie = System.Text.Encoding.UTF8.GetString(cookieData);
                cookieContainer.Add(new Uri("http://volontar.polisen.se/"), new Cookie("PHPSESSID", sessionCookie));

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
                            var reportUrl = MyAssignmentReportHelper.GetReportUrl(handler, navigationInfo, areaIndex);
                            if (string.IsNullOrEmpty(reportUrl))
                            {
                                return false;
                            }

                            var result = MyAssignmentReportHelper.PostReport(
                                handler, reportUrl,
                                name, email,
                                assignmentOrDate,
                                feedback1, feedback2, feedback3
                            );

                            return result;
                        }
                    }
                }
            }

            return false;
        }
    }
}