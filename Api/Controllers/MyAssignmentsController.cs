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
            this.Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, post-check=0, pre-check=0");

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
            this.Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, post-check=0, pre-check=0");

            // TODO: 1. Sanity checking
            // TODO: 2. Validate login
            // TODO: 3. Return specific assignmet
            return new Assignment
            {
            };
        }
    }
}