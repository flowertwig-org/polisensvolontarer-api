using System.Collections.Generic;
using System.Linq;
using Api.Contracts;
using Api.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class AssignmentController : Controller
    {
        private AppSettings _appSettings;
        public AssignmentController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        // GET api/Assignment/{id}
        [HttpGet]
        //[ResponseCache(VaryByQueryKeys = new[] { "key" }, Duration = 60)]
        public JsonResult Get(string key, string cookieFailKey = null)
        {
            try
            {
                this.Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, post-check=0, pre-check=0");

                var keyInfo = new CookieFailKeyInfo(cookieFailKey);

                List<Assignment> list = AvailableAssignmentsHelper.GetAvailableAssignments(this.HttpContext, keyInfo);
                var item = list.FirstOrDefault(a => a.Id == key);
                if (item == null)
                {
                    return Json(null);
                }

                var assignment = AssignmentDetailHelper.GetAssignmentDetail(HttpContext, keyInfo, _appSettings, item);
                if (assignment == null)
                {
                    return Json(null);
                }

                return Json(new AssignmentDetail
                {
                    Id = item.Id,
                    Name = item.Name,
                    Category = item.Category,
                    Date = item.Date,
                    Area = item.Area,
                    Description = assignment.Description,
                    Time = assignment.Time,
                    ContactInfo = assignment.ContactInfo,
                    MeetupTime = assignment.MeetupTime,
                    MeetupPlace = assignment.MeetupPlace,
                    LastRequestDate = assignment.LastRequestDate,
                    GoogleCalendarEventUrl = assignment.GoogleCalendarEventUrl,
                    WantedNumberOfPeople = assignment.WantedNumberOfPeople,
                    CurrentNumberOfPeople = assignment.CurrentNumberOfPeople,
                    InterestsFormUrl = assignment.InterestsFormUrl,
                    InterestsValues = assignment.InterestsValues
                });
            }
            catch (System.Exception ex)
            {
                return Json(new Assignment { Name = ex.Message });
            }
        }
    }
}