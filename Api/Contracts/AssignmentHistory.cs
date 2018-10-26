using System;
namespace Api.Contracts
{
    public class AssignmentHistory
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Area { get; set; }
        public string Status { get; set; }

        public AssignmentHistory()
        {
        }
    }
}
