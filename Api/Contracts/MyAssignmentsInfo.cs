using System.Collections.Generic;

namespace Api.Contracts
{
    public class MyAssignmentsInfo
    {
        public IEnumerable<Assignment> Interests
        {
            get;
            set;
        }

        public IEnumerable<Assignment> Confirms
        {
            get;
            set;
        }

        public IEnumerable<Assignment> Reservations
        {
            get;
            set;
        }

        public MyAssignmentsInfo()
        {
        }
    }
}