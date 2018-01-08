using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using Api.Contracts;

namespace Api.Helpers
{
    public class AssignmentDetailHelper
    {
        public static AssignmentDetail GetAssignmentDetailFromUrl(HttpClientHandler handler, Assignment baseAssignment, string webSiteUrl)
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
                        string meetupTime = null;
                        string meetupPlace = null;
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
                                .Replace("</p>", "<br>")
                                .Replace("<br>&nbsp;<br>", "<br>");
                            
                            if (value.IndexOf("Kategori:", System.StringComparison.Ordinal) != -1) {
                                continue;
                            }

                            if (value.IndexOf("Uppdragsbeskrivning:", System.StringComparison.Ordinal) != -1) {
                                description = value.Replace("Uppdragsbeskrivning:", "")
                                                   .Replace("<p style=\"margin: 0cm 0cm 0.0001pt\">", "");

                                while (description.IndexOf("<br>", System.StringComparison.Ordinal) == 0)
                                {
                                    description = description.Substring(4);
                                }
                                continue;
                            }

                            if (value.IndexOf("Tid:", System.StringComparison.Ordinal) != -1)
                            {
                                // TODO: Fixa denna
                                time = value.Replace("Tid:", "");
                                time = time.Replace("<br>", "");
                                continue;
                            }

                            if (value.IndexOf("Samordnas av:", System.StringComparison.Ordinal) != -1) {
                                // TODO: Fixa denna
                                contactInfo += value.Replace("Samordnas av:", "<b>Samordnas av:</b>");

                                var index = group.Index + group.Length;
                                value = pageContent.Substring(index, 300)
                                                                   .Replace("</div>", "")
                                                                   .Replace("\t", "")
                                                                   .Replace("</tr>", "")
                                                                   .Replace("</td>", "")
                                                                   .Replace("<tr>", "")
                                                                   .Replace("\r\n", "")
                                                                   .Replace("<td class=\"arial13bold Ofc401641\" valign=\"top\">", "")
                                                                   .Replace("<td class=\"arial13bold O1621a455\" valign=\"top\">", "")
                                                                   .Replace("<td class=\"NORMAL O5d0fe1aa\" valign=\"top\">", "");
                                index = value.IndexOf('<');
                                if (index > 0){
                                    value = value.Substring(0, index);
                                }
                                contactInfo += value;
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

                                // 20181030
                                if (lastRequestDate.Length == 8) {
                                    var temp = lastRequestDate.Substring(0,4) + "-" + lastRequestDate.Substring(4, 2) + "-" + lastRequestDate.Substring(6,2);
                                    DateTime test;
                                    if (DateTime.TryParse(temp, out test)) {
                                        lastRequestDate = temp;
                                    }
                                }
                                continue;
                            }

                            if (value.IndexOf("Tid och plats uts&auml;ttning:", System.StringComparison.Ordinal) != -1)
                            {
                                // TODO: Fixa denna
                                meetupTimeAndPlace = value.Replace("Tid och plats uts&auml;ttning:", "");
                                meetupTimeAndPlace = meetupTimeAndPlace.Replace("<br>", "");
                                var meetupPair = meetupTimeAndPlace.Split(new char[] {'|'});
                                if (meetupPair.Length == 2) {
                                    meetupTime = meetupPair[0].Trim();
                                    meetupPlace = meetupPair[1].Trim();
                                }
                                continue;
                            }

                            if (value.IndexOf("Hittills antal anm&auml;lda:", System.StringComparison.Ordinal) != -1)
                            {
                                // TODO: Fixa denna
                                var index = group.Index + group.Length;
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

                            name = value;
                        }

                        var startTime = "";
                        var endTime = "";

                        var date = baseAssignment.Date.Replace("-", "");
                        var startAndEndTime = time.Replace(":", "")
                                                  .Replace(".", "")
                                                  .Replace("Kl", "")
                                                  .Split(new char[] {'-',' '}, System.StringSplitOptions.RemoveEmptyEntries);
                        if (startAndEndTime.Length == 2) {
                            startTime = "T" + startAndEndTime[0];
                            endTime = "T" + startAndEndTime[1];
                        }

                        var location = "";
                        if (meetupPlace != null) {
                            location = $"&location={WebUtility.UrlEncode(meetupPlace)}";
                        }

                        var url = webSiteUrl + "/restricted/assignment/?key=" + baseAssignment.Id;
                        var details = $"&details={WebUtility.UrlEncode(url)}";

                        var title = $"&text={WebUtility.UrlEncode(baseAssignment.Name)}";

                        var googleCalendarEventUrl = $"https://www.google.com/calendar/render?action=TEMPLATE{title}&dates={date}{startTime}00/{date}{endTime}00{details}{location}&sf=true&output=xml";

                        return new AssignmentDetail
                        {
                            Description = description,
                            Name = name,
                            ContactInfo = contactInfo,
                            MeetupTime = meetupTime,
                            MeetupPlace = meetupPlace,
                            CurrentNumberOfPeople = currentNumberOfPeople,
                            WantedNumberOfPeople = wantedNumberOfPeople,
                            Time = time,
                            LastRequestDate = lastRequestDate,
                            GoogleCalendarEventUrl = googleCalendarEventUrl
                        };
                    }
                }
            }
            catch (System.Exception ex)
            {
                return new AssignmentDetail { Description = "Unknown Error: " + ex };
            }
        }
    }
}
