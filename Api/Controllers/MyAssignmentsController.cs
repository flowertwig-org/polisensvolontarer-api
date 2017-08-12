using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Api.Contracts;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class MyAssignmentsController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<Assignment> Get()
        {
            // TODO: 1. Sanity checking
            // TODO: 2. Validate login
            // TODO: 3. Return my assignmets
            return new Assignment[] {
                new Assignment{ }
            };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public Assignment Get(int id)
        {
            // TODO: 1. Sanity checking
            // TODO: 2. Validate login
            // TODO: 3. Return specific assignmet
            return new Assignment
            {
            };
        }
    }
}