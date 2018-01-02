using System.Net.Http;
using System.Text.RegularExpressions;
using Api.Contracts;

namespace Api.Helpers
{
    public class AssignmentDetailHelper
    {
        public static AssignmentDetail GetAssignmentDetailFromUrl(HttpClientHandler handler, Assignment baseAssignment)
        {
            try
            {

                var idData = System.Convert.FromBase64String(baseAssignment.Id.Replace("_", "/").Replace("-", "="));
                var detailUrl = System.Text.Encoding.UTF8.GetString(idData);

                HttpClient client = new HttpClient(handler);
                using (var response = client.GetAsync("http://volontar.polisen.se/" + detailUrl).Result)
                {
                    using (var responseContent = response.Content)
                    {
                        var pageContent = responseContent.ReadAsStringAsync().Result;

                        //return new AssignmentDetail { Name = "PageContent", Description = pageContent };
                        string description = null;
                        string name = null;
                        string lastRequestDate = null;
                        string contactInfo = "";
                        string meetupTimeAndPlace = null;
                        string currentNumberOfPeople = null;
                        string wantedNumberOfPeople = null;
                        string time = null;

                        string pattern = "<div class\\=\\\"arial13bold\\\">(?<content>.+)<\\/div>";
                        var matches = Regex.Matches(pageContent, pattern);
                        foreach (Match match in matches)
                        {
                            if (!match.Success)
                            {
                                continue;
                            }

                            var group = match.Groups["content"];
                            if (!group.Success) {
                                continue;
                            }

                            var value = group.Value;

                            value = value
                                .Replace("<p>", "")
                                .Replace("<font color=\"000000\">", "")
                                .Replace("<strong>", "")
                                .Replace("</strong>", "")
                                .Replace("</font>", "")
                                .Replace("</p>", "<br><br>");
                            
                            if (value.IndexOf("Kategori:", System.StringComparison.Ordinal) != -1) {
                                continue;
                            }

                            if (value.IndexOf("Uppdragsbeskrivning:", System.StringComparison.Ordinal) != -1) {
                                description = value.Replace("Uppdragsbeskrivning:", "");

                                while (description.IndexOf("<br>", System.StringComparison.Ordinal) == 0)
                                {
                                    description = description.Substring(4);
                                }
                                continue;
                            }

                            if (value.IndexOf("Samordnas av:", System.StringComparison.Ordinal) != -1) {
                                // TODO: Fixa denna
                                contactInfo += value.Replace("Samordnas av:", "<b>Samordnas av:</b>");
                                continue;
                            }

                            if (value.IndexOf("Volont&auml;ransvarig:", System.StringComparison.Ordinal) != -1)
                            {
                                // TODO: Fixa denna
                                contactInfo += value.Replace("Volont&auml;ransvarig:", "<b>Volont&auml;ransvarig:</b>");
                                continue;
                            }

                            if (value.IndexOf("Sista anm&auml;lningsdag:", System.StringComparison.Ordinal) != -1)
                            {
                                // TODO: Fixa denna
                                lastRequestDate = value.Replace("Sista anm&auml;lningsdag:", "");
                                lastRequestDate = lastRequestDate.Replace("<br>", "");
                                continue;
                            }

                            if (value.IndexOf("Tid och plats uts&auml;ttning:", System.StringComparison.Ordinal) != -1)
                            {
                                // TODO: Fixa denna
                                meetupTimeAndPlace = value; //.Replace("Tid och plats uts&auml;ttning:", "");
                                //meetupTimeAndPlace = meetupTimeAndPlace.Replace("<br>", "");
                                continue;
                            }

                            if (value.IndexOf("Hittills antal anm&auml;lda:", System.StringComparison.Ordinal) != -1)
                            {
                                // TODO: Fixa denna
                                var index = group.Index + group.Length;
                                //currentNumberOfPeople = value; //.Replace("Tid och plats uts&auml;ttning:", "");
                                //meetupTimeAndPlace = meetupTimeAndPlace.Replace("<br>", "");
                                value = pageContent.Substring(index, 200)
                                                                   .Replace("</div>", "")
                                                                   .Replace("\t", "")
                                                                   .Replace("</tr>", "")
                                                                   .Replace("</td>", "")
                                                                   .Replace("<tr>", "")
                                                                   .Replace("\r\n", "")
                                                                   .Replace("<td class=\"arial13bold Ofc401641\" valign=\"top\">", "");
                                index = value.IndexOf("<", System.StringComparison.Ordinal);
                                if (index != -1) {
                                    currentNumberOfPeople = value.Substring(0, index);
                                }
                                continue;
                            }
                            if (value.IndexOf("&Ouml;nskat antal volont&auml;rer:", System.StringComparison.Ordinal) != -1)
                            {
                                // TODO: Fixa denna
                                wantedNumberOfPeople = value.Replace("&Ouml;nskat antal volont&auml;rer:", "");
                                wantedNumberOfPeople = wantedNumberOfPeople.Replace("<br>", "");
                                continue;
                            }
                            if (value.IndexOf("Tid:", System.StringComparison.Ordinal) != -1)
                            {
                                // TODO: Fixa denna
                                time = value.Replace("Tid:", "");
                                time = time.Replace("<br>", "");
                                continue;
                            }

                            name = value;
                        }

                        return new AssignmentDetail
                        {
                            Description = description,
                            Name = name,
                            ContactInfo = contactInfo,
                            MeetupTimeAndPlace = meetupTimeAndPlace,
                            CurrentNumberOfPeople = currentNumberOfPeople,
                            WantedNumberOfPeople = wantedNumberOfPeople,
                            Time = time
                        };
                    }
                }
                return null;
            }
            catch (System.Exception ex)
            {
                return new AssignmentDetail { Name = "ERROR", Description = ex.Message };
            }
        }
    }
}
