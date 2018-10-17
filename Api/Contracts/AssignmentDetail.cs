using System.Collections.Generic;

namespace Api.Contracts
{
    public class AssignmentDetail : Assignment
    {
        public string Time { get; set; }
        public string WantedNumberOfPeople
        {
            get;
            set;
        }
        public string CurrentNumberOfPeople
        {
            get;
            set;
        }
        public string ContactInfo
        {
            get;
            set;
        }

        public string GoogleCalendarEventUrl { get; set; }
        public string Description { get; set; }
        public string MeetupTime { get; set; }
        public string MeetupPlace { get; set; }
        public string LastRequestDate { get; set; }
        public string InterestsFormUrl { get; set; }
        public List<KeyValuePair<string, string>> InterestsValues { get; set; }

        public override string ToString()
        {
            return string.Format("[AssignmentDetail: Time={0}, WantedNumberOfPeople={1}, CurrentNumberOfPeople={2}, ContactInfo={3}, Description={4}, Name={5}, Date={6} Category={7}, Area={8}, Id={9}, GoogleCalendarEventUrl={10}]", Time, WantedNumberOfPeople, CurrentNumberOfPeople, ContactInfo, Description, Name, Date, Category, Area, Id, GoogleCalendarEventUrl);
        }
    }
}