using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Api.Contracts
{
    public class AssignmentsHistoryInfo
    {
        private string content;

        public List<AssignmentHistory> Assignments { get; set; }

        public AssignmentsHistoryInfo(string content)
        {
            this.Assignments = new List<AssignmentHistory>();
            this.content = content;
            ParseContent(content);
        }

        public void ParseContent(string content)
        {
            try
            {
                string date = null;
                string name = null;
                string status = null;

                var inTheFuture = DateTime.Today.AddHours(23).AddMinutes(59);

                var columnMatches = Regex.Matches(content, "<td style=\"padding-left:10px;\">(?<columnData>[^<]+)");
                foreach (Match columnMatch in columnMatches)
                {
                    if (columnMatch.Success)
                    {
                        if (string.IsNullOrEmpty(date))
                        {
                            date = columnMatch.Groups["columnData"].Value;
                        }
                        else if (string.IsNullOrEmpty(name))
                        {
                            name = columnMatch.Groups["columnData"].Value;
                        }
                        else if (string.IsNullOrEmpty(status))
                        {
                            status = columnMatch.Groups["columnData"].Value;
                        }

                        if (!string.IsNullOrEmpty(date) && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(status))
                        {
                            if (DateTime.TryParse(date.Trim(), out DateTime dateTime))
                            {
                                if (dateTime <= inTheFuture)
                                {
                                    Assignments.Add(new AssignmentHistory
                                    {
                                        Name = name.Trim(),
                                        Date = dateTime,
                                        Status = status.Trim()
                                    });
                                }
                            }
                            date = null;
                            name = null;
                            status = null;
                        }
                    }
                }

            }
            catch (System.Exception ex)
            {
                Assignments.Add(new AssignmentHistory { Name = ex.Message });
            }
        }

    }
}
