using System.Linq;

namespace Api.Contracts
{
    public class AvailableAssignmentsResult
    {
        public int TotalNumberOfItems { get; set; }
        public int FilteredNofItems { get; set; }
        public IGrouping<string, Assignment>[] Items { get; set; }
        public int NextStartIndex { get; set; }

        public AvailableAssignmentsResult()
        {
        }
    }
}
