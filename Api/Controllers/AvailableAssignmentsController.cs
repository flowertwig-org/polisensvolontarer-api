using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Api.Contracts;
using System.Linq;
using System;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class AvailableAssignmentsController : Controller
    {
        // GET api/values
        [HttpGet]
        public IGrouping<string, Assignment>[] Get()
        {
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
			}

            // TODO: 3. Return available assignmets
            var groupedList = list.GroupBy(GroupByDay);
            return groupedList.ToArray();
        }

        private string GroupByDay(Assignment assignment)
        {
            DateTime date;
            if(DateTime.TryParse(assignment.Date, out date)) {
                return date.ToString("yyyy-MM-dd");
            }
            return "";
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public Assignment Get(int id)
        {
            // TODO: 1. Sanity checking
            // TODO: 2. Validate login
            // TODO: 3. Return specific assignmet
            return new Assignment { };
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]int id)
        {
            // TODO: 1. Sanity checking
            // TODO: 2. Validate login
            // TODO: 3. Sign up for specific assignmet
        }
    }
}