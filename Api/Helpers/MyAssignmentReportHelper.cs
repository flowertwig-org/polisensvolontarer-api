using System.Collections.Generic;
using System.Net.Http;
using Api.Contracts;

namespace Api.Helpers
{
    public class MyAssignmentReportHelper
    {
        public static string GetReportUrl(HttpClientHandler handler, NavigationInfo navigationInfo, int areaIndex) {
			HttpClient client = new HttpClient(handler);

            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "VolontarPortal/1.0");

            using (var response = client.GetAsync("http://volontar.polisen.se/" + navigationInfo.MyAssignmentsReportTypesUrl).Result)
			{
				using (var responseContent = response.Content)
				{
					var content = responseContent.ReadAsStringAsync().Result;

                    var areas = AvailableAssignmentsHelper.GetAreas();
                    var areaCount = areas.Count;
                    if (areaIndex < 0 || areaIndex >= areaCount)
                    {
                        return null;
                    }

                    var areaName = areas[areaIndex];
                    // Korrigera "area" namnet, eftersom webbplatsen inte har samma namn som i uppdragen
                    switch (areaName)
                    {
                        case "Gränspolisenheten":
                            areaName = "Arlanda";
                            break;
                        case "Övrig":
                            areaName = "Övriga";
                            break;
                    }

                    areaName = areaName.Replace("Ö", "&Ouml;");
                    areaName = areaName.Replace("ö", "&ouml;");
                    areaName = areaName.Replace("ä", "&auml;");

                    // Get person name
                    //<span class="Inloggad"><strong><strong>(.*)<\/strong><\/strong>

                    var linkNameIndex = content.IndexOf(areaName, System.StringComparison.Ordinal);
                    if (linkNameIndex == -1)
                    {
                        return null;
                    }

                    content = content.Substring(0, linkNameIndex);

                    var linkStartTagIndex = content.LastIndexOf("<a", System.StringComparison.Ordinal);
                    if (linkStartTagIndex == -1)
                    {
                        return null;
                    }

                    content = content.Substring(linkStartTagIndex);

                    var urlMatch = System.Text.RegularExpressions.Regex.Match(content, "<a href=\"(?<url>[^\"]+)\"");
                    if (urlMatch.Success)
                    {
                        var urlGroup = urlMatch.Groups["url"];
                        if (urlGroup.Success)
                        {
                            return "http://volontar.polisen.se/" + urlGroup.Value.Replace("../", "");
                        }
                    }
                }
			}
            return null;
		}

        public static bool PostReport(
            HttpClientHandler handler, string reportUrl,
            string name, string email,
            string datum,
            string feedback1,
            string feedback2,
            string feedback3)
		{
            // ta fram ref_page = web_page-s1/299:
            // kör nedan regexp på url till uppdrags rapport:
            // \/([0-9]{2,4})\/
            // ta värdet du får och lägg in i strängen:
            // web_page-s1/<värde>

            var match = System.Text.RegularExpressions.Regex.Match(reportUrl, "\\/(?<pageId>[0-9]{2,4})\\/");
            if (match.Success)
            {
                var group = match.Groups["pageId"];
                if (group.Success)
                {
                    HttpClient client = new HttpClient(handler);

                    client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "VolontarPortal/1.0");

                    var httpContent = new FormUrlEncodedContent(new List<KeyValuePair<string, string>> {
                        new KeyValuePair<string, string>( "namn", name),
                        new KeyValuePair<string, string>( "mail", email),
                        new KeyValuePair<string, string>( "datum", datum),
                        new KeyValuePair<string, string>( "genom", feedback1), // textarea
                        new KeyValuePair<string, string>( "efter", feedback2), // textarea
                        new KeyValuePair<string, string>( "igen", feedback3), // textarea
                        new KeyValuePair<string, string>( "ref_page", "web_page-s1/" + group.Value), // Måste hämta värde från sida
                        new KeyValuePair<string, string>( "sd_formcount", "1"),
                        new KeyValuePair<string, string>( "submit_4_5", "Submit")
                    });
                    using (var response = client.PostAsync(reportUrl, httpContent).Result)
                    {
                        using (var responseContent = response.Content)
                        {
                            var content = responseContent.ReadAsStringAsync().Result;

                            return IsThanksContent(content);
                        }
                    }
                }

            }

            return false;
		}

        private static bool IsThanksContent(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return false;
            }
            return content.IndexOf("<title>Tack</title>", System.StringComparison.Ordinal) != -1;
        }
    }
}