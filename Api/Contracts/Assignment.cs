using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Contracts
{
    public class Assignment
    {
        public string Name { get; set; }
        public string Category { get; set; }

        public string Area { get; set; }
        public string Description { get; set; }
    }
}
