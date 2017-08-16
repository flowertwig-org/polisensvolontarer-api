using System.Collections.Generic;

namespace Api.Contracts
{
    public class AvailableAssignmentsInfo
    {
        private string content;

        public List<Assignment> Assignments
        {
            get;
            set;
        }

        public AvailableAssignmentsInfo(string content)
        {
            Assignments = new List<Assignment>();
            this.content = content;
        }
    }
}