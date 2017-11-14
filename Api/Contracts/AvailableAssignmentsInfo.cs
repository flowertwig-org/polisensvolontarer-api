using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Api.Contracts
{
    public class AvailableAssignmentsInfo
    {
        private string content;

        public List<Assignment> Assignments
        {
            get;
            set;
        }

        public AvailableAssignmentsInfo(string content)
        {
            Assignments = new List<Assignment>();
            this.content = content;
            ParseContent(content);
        }

        public void ParseContent(string content) {
            try
            {
                var rowMatches = Regex.Matches(content, "23px");
                foreach (Match rowMatch in rowMatches)
                {
                    if (rowMatch.Success)
                    {
                        var startTagStartIndex = rowMatch.Index + rowMatch.Length;
                        var endTagStartIndex = content.IndexOf("</tr>", rowMatch.Index, System.StringComparison.Ordinal);
                        var itemContent = content.Substring(startTagStartIndex, endTagStartIndex - startTagStartIndex);

                        string name = "";
                        string link = "";
                        string place = "";
                        string date = "";
                        string assignmentType = "";

                        var index = 0;
                        var columnMatches = Regex.Matches(itemContent, "<td");
                        foreach (Match columnMatch in columnMatches)
                        {
                            var columnStartTagStartIndex = columnMatch.Index + columnMatch.Length;
                            var columnEndTagStartIndex = itemContent.IndexOf("</td>", columnMatch.Index, System.StringComparison.Ordinal);
                            var columnContent = itemContent.Substring(columnStartTagStartIndex, columnEndTagStartIndex - columnStartTagStartIndex);
                            switch (index)
                            {
                                case 1:
                                    var dateIndex = columnContent.IndexOf('>');
                                    if (dateIndex != -1)
                                    {
                                        dateIndex++;
                                        date = columnContent.Substring(dateIndex);
                                    }
                                    break;
                                case 3:
                                    // link
                                    // name
                                    name = columnContent;
                                    var linkSearchString = "<a href=\"";
                                    var linkIndex = columnContent.IndexOf(linkSearchString, System.StringComparison.Ordinal);
                                    if (linkIndex != -1)
                                    {
                                        linkIndex += linkSearchString.Length;
                                        var linkEndIndex = columnContent.IndexOf("\">", linkIndex, System.StringComparison.Ordinal);
                                        if (linkEndIndex != -1)
                                        {
                                            link = columnContent.Substring(linkIndex, linkEndIndex - linkIndex);
                                            link = link.Replace("../../..", "");

                                            linkEndIndex += 2;
                                            var nameIndex = columnContent.IndexOf("</a>", linkEndIndex, System.StringComparison.Ordinal);
                                            if (nameIndex != -1)
                                            {
                                                name = columnContent.Substring(linkEndIndex, nameIndex - linkEndIndex);
                                            }
                                        }
                                    }
                                    break;
                                case 5:
                                    var placeIndex = columnContent.IndexOf('>');
                                    if (placeIndex != -1)
                                    {
                                        placeIndex++;
                                        place = columnContent.Substring(placeIndex);
                                    }
                                    break;
                                case 7:
                                    var typeIndex = columnContent.IndexOf('>');
                                    if (typeIndex != -1)
                                    {
                                        typeIndex++;
                                        assignmentType = columnContent.Substring(typeIndex);
                                    }
                                    break;
                                default:
                                    break;
                            }
                            index++;
                        }

                        Assignments.Add(new Assignment
                        {
                            Name = name,
                            Area = place,
                            Category = assignmentType,
                            Date = date,
                            Link = link
                        });
                    }
                }

            }
            catch (System.Exception ex)
            {
                Assignments.Add(new Assignment{ Name = ex.Message });
            }
        }
    }
}