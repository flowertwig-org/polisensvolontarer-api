using System.Collections.Generic;
using System.Linq;
using Api.Contracts;
using Api.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class AssignmentController : Controller
    {
        // GET api/Assignment/{id}
        [HttpGet]
        //[ResponseCache(VaryByQueryKeys = new[] { "key" }, Duration = 60)]
        public AssignmentDetail Get(string key)
        {
            // TODO: 1. Sanity checking
            // TODO: 2. Validate login
            List<Assignment> list = AvailableAssignmentsHelper.GetAvailableAssignments(this.HttpContext);
            var item = list.FirstOrDefault(a => a.Id == key);
            if (item == null)
            {
                return null;
            }

            return item as AssignmentDetail;
        }
    }
}