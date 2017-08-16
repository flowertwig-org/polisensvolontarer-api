namespace Api.Contracts
{
    public class NavigationInfo
    {
        public NavigationInfo()
        {

        }

        public NavigationInfo(string pageContent)
        {

        }

        public string AvailableAssignmentsUrl
        {
            get;
            set;
        }

        public string LogoutUrl
        {
            get;
            set;
        }

        public string MyAssignmentsUrl
        {
            get;
            set;
        }

        public string MyAssignmentsReportTypesUrl
        {
            get;
            set;
        }

        public string CoordinatorsUrl
        {
            get;
            set;
        }

        public string ContactFormUrl
        {
            get;
            set;
        }

        public override string ToString()
        {
            return string.Format("[NavigationInfo: AvailableAssignmentsUrl={0}, LogoutUrl={1}, MyAssignmentsUrl={2}, MyAssignmentsReportTypesUrl={3}, CoordinatorsUrl={4}, ContactFormUrl={5}]", AvailableAssignmentsUrl, LogoutUrl, MyAssignmentsUrl, MyAssignmentsReportTypesUrl, CoordinatorsUrl, ContactFormUrl);
        }
    }
}