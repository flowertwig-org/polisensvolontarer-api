using System.Collections.Generic;

namespace Api.Contracts
{
    public class AvailableAssignmentFilterSettings
    {
        public List<int> AlwaysShowTypes
        {
            get;
            set;
        }

        public List<int> NeverShowTypes
        {
            get;
            set;
        }

        public List<int> HideWorkDayTypes
        {
            get;
            set;
        }

        public List<int> HideWeekendTypes
        {
            get;
            set;
        }

        public List<int> NeverShowAreas
        {
            get;
            set;
        }

        public List<int> AlwaysShowAreas
        {
            get;
            set;
        }

        public List<int> NeverShowSpecTypes
        {
            get;
            set;
        }

        public AvailableAssignmentFilterSettings()
        {
            AlwaysShowTypes = new List<int>();
            NeverShowTypes = new List<int>();
            HideWorkDayTypes = new List<int>();
            HideWeekendTypes = new List<int>();
            NeverShowAreas = new List<int>();
            AlwaysShowAreas = new List<int>();
            NeverShowSpecTypes = new List<int>();
        }
    }
}