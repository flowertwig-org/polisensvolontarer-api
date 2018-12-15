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

        public class ReportResult
        {
            public bool IsSuccess { get; set; }
            //public string Name { get; set; }
            //public string Email { get; set; }
        }

        // POST api/AvailableAssignments
        [HttpPost]
        public ReportResult Post(
            [FromForm]bool anonymous,
            [FromForm]string assignmentOrDate, [FromForm]int areaIndex,
            [FromForm]string feedback1, [FromForm]string feedback2, [FromForm]string feedback3, string cookieFailKey = null)
        {
            ReportResult reportResult = new ReportResult();

            var keyInfo = new CookieFailKeyInfo(cookieFailKey);

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
                                return reportResult;
                            }

                            var reportInfo = MyAssignmentReportHelper.GetReportActionUrlAndUserName(handler, reportUrl);
                            var actionUrl = reportInfo.ActionUrl;
                            if (string.IsNullOrEmpty(actionUrl))
                            {
                                return reportResult;
                            }

                            var name = "";
                            var email = "";
                            if (!anonymous)
                            {
                                name = reportInfo.UserFullName ?? "";
                                email = MyAssignmentReportHelper.GetUserEmail(HttpContext, keyInfo, _appSettings) ?? "";
                            }
                            else
                            {
                                name = "Användaren har valt att vara anonym";
                            }

                            //reportResult.Name = name;
                            //reportResult.Email = email;

                            var result = MyAssignmentReportHelper.PostReport(
                                handler, actionUrl,
                                name, email,
                                assignmentOrDate,
                                feedback1, feedback2, feedback3
                            );

                            reportResult.IsSuccess = result;

                            return reportResult;
                        }
                    }
                }
            }

            return reportResult;
        }
    }
}