using System;
namespace Api.Contracts
{
    public class AssignmentDetail : Assignment
    {
        public string Time { get; set; }
        public int WantedNumberOfPeople
        {
            get;
            set;
        }
        public int CurrentNumberOfPeople
        {
            get;
            set;
        }
        public string ContactInfo
        {
            get;
            set;
        }
        public string Description { get; set; }

        public AssignmentDetail()
        {
        }
    }
}
