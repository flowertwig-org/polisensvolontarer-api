using System;
using System.Collections.Generic;
using Api.Contracts;
using Microsoft.AspNetCore.Http;

namespace Api.Helpers
{
    public class AvailableAssignmentsHelper
    {
        public static string GroupByDay(Assignment assignment)
        {
            DateTime date;
            if (DateTime.TryParse(assignment.Date, out date))
            {
                return date.ToString("yyyy-MM-dd");
            }
            return "";
        }


        public static List<Assignment> GetAvailableAssignments(HttpContext httpContext)
        {
            var list = new List<Assignment>();
            try
            {
                byte[] data;
                if (httpContext.Session.TryGetValue("AvailableAssignments", out data))
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

            return list;
        }
    }
}
