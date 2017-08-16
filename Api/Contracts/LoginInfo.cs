namespace Api.Contracts
{
    public class LoginInfo
    {
		public bool Status { get; set; }
        public NavigationInfo MainNavigation
        {
            get;
            set;
        }
        public AvailableAssignmentsInfo AvailableAssignments { get; set; }

        public override string ToString()
        {
            return string.Format("[LoginInfo: Status={0}, MainNavigation={1}, AvailableAssignments={2}]", Status, MainNavigation, AvailableAssignments);
        }

	}
}
