using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Api.Contracts;
using System.Linq;
using Api.Helpers;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class AvailableAssignmentsController : Controller
    {
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
        public void Post([FromBody]string id)
        {
            // TODO: 1. Sanity checking
            // TODO: 2. Validate login
            // TODO: 3. Sign up for specific assignmet
        }
    }
}