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
        public string Description { get; set; }

        public string MeetupTimeAndPlace { get; set; }

        public override string ToString()
        {
            return string.Format("[AssignmentDetail: Time={0}, WantedNumberOfPeople={1}, CurrentNumberOfPeople={2}, ContactInfo={3}, Description={4}, Name={5}, Date={6} Category={7}, Area={8}, Id={9}]", Time, WantedNumberOfPeople, CurrentNumberOfPeople, ContactInfo, Description, Name, Date, Category, Area, Id);
        }
    }
}
