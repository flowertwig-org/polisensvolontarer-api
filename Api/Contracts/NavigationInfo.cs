using System;

namespace Api.Contracts
{
    public class NavigationInfo
    {
        private string content;

        public NavigationInfo()
        {

        }

        public NavigationInfo(string pageContent)
        {
            this.content = pageContent;
            ParseContent(pageContent);
        }

        private string GetLink(string linkText) {
            var regExp = "<a href=\"(?<link>[^\"]+)\"[^>]+>" + linkText + "<\\/a>";
            var match = System.Text.RegularExpressions.Regex.Match(this.content, regExp);
            var group = match.Groups["link"];
            if (group.Success)
            {
                return group.Value.Replace("../", "");
            }
            return null;
        }

        private void ParseContent(string pageContent)
        {
            this.MyAssignmentsUrl = GetLink("Mina uppdrag");
            this.MyAssignmentsReportTypesUrl = GetLink("Uppdragsrapport");
            this.ContactFormUrl = GetLink("Kontakta oss");
            this.AvailableAssignmentsUrl = GetLink("Aktuella uppdrag");
            this.CoordinatorsUrl = GetLink("Samordnare");
            this.CoordinatorsUrl = GetLink("Samordnare");
            this.ChangePasswordUrl = GetLink("Ändra lösenord");

            var regExp = "href=\"(?<logoutLink>[^\"]+logout=1)\"";
            var match = System.Text.RegularExpressions.Regex.Match(this.content, regExp);
            var group = match.Groups["logoutLink"];
            if (group.Success)
            {
                this.LogoutUrl = group.Value.Replace("../", "");
            }
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
        public string ChangePasswordUrl { get; set; }
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