using System;

namespace Api.Contracts
{
    public class CookieFailKeyInfo
    {
        public bool IsVaild { get; set; }
        public string Key { get; set; }
        public string AvailableAssignmentsUrl { get; set; }

        public CookieFailKeyInfo(string cookieFailKey)
        {
            try
            {
                // https://test-polisens-volontarer-api.azurewebsites.net/api/login?cookieFailKey=694b333689169e7a425686f5878f1d03;MS4wLjEuMC8yLzEvP2w9MzI0NiZzPTMyNDY=
                if (!string.IsNullOrEmpty(cookieFailKey))
                {
                    var sections = cookieFailKey.Split(";", System.StringSplitOptions.RemoveEmptyEntries);
                    if (sections.Length == 2)
                    {
                        Key = sections[0];

                        var urlData = Convert.FromBase64String(sections[1].Replace("-", "+").Replace("_", "/"));
                        AvailableAssignmentsUrl = System.Text.Encoding.UTF8.GetString(urlData);

                        IsVaild = true;

                        //try
                        //{
                        //    var url = new System.Uri("http://volontar.polisen.se" + AvailableAssignmentsUrl);
                        //    IsVaild = true;
                        //}
                        //catch (System.Exception ex)
                        //{

                        //}
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public static string ToKey(string value, string availableAssignmentsUrl)
        {
            var urlData = System.Text.Encoding.UTF8.GetBytes(availableAssignmentsUrl);
            return value + ";" + Convert.ToBase64String(urlData).Replace("+", "-").Replace("/", "_");
        }

        public override string ToString()
        {
            return IsVaild + ";" + Key + ";" + AvailableAssignmentsUrl;
        }
    }
}