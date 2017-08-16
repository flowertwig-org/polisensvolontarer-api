using System;
using System.Collections.Generic;
using System.Net.Http;
using Api.Contracts;
using Microsoft.AspNetCore.Http;

namespace Api.Helpers
{
    public class LoginHelper
    {
        public LoginHelper()
        {
        }

        public static bool IsLoginContent(string content) {
            if (string.IsNullOrEmpty(content)) {
                return false;
            }
            return !content.Contains("<title>LOGIN</title>");
        }

        public static string GetLoginUrl(HttpClientHandler handler) {
			HttpClient client = new HttpClient(handler);
			using (var response = client.GetAsync("http://volontar.polisen.se").Result)
			{
				using (var responseContent = response.Content)
				{
					var content = responseContent.ReadAsStringAsync().Result;

					var formActionStartPattern = "<form action=\"";
					var formActionStartIndex = content.IndexOf(formActionStartPattern);
					formActionStartIndex += formActionStartPattern.Length;
					var formActionEndIndex = content.IndexOf("\"", formActionStartIndex);
                    var cookies = response.Headers.GetValues("Set-Cookie");
					var action = content.Substring(formActionStartIndex, formActionEndIndex - formActionStartIndex);
					action = "http://volontar.polisen.se/" + action.TrimStart('.', '/');
					return action;
				}
			}
		}

		public static LoginInfo Login(HttpClientHandler handler, string loginUrl, string username, string password)
		{
			HttpClient client = new HttpClient(handler);
            var httpContent = new FormUrlEncodedContent(new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>( "login", username),
                new KeyValuePair<string, string>( "password", password),
				new KeyValuePair<string, string>( "ref_page", "eb_page-s1/12"),
				new KeyValuePair<string, string>( "itemid", ""),
				new KeyValuePair<string, string>( "sd_formcount", "1"),
                new KeyValuePair<string, string>( "submit_12_0", "Submit")
            });
			using (var response = client.PostAsync(loginUrl, httpContent).Result)
			{
				using (var responseContent = response.Content)
				{
					var content = responseContent.ReadAsStringAsync().Result;
                    var info = new LoginInfo();
                    if (!IsLoginContent(content)) {
                        return info;
                    }else  {
                        info.Status = true;

                        info.MainNavigation = new NavigationInfo(content);
                        info.AvailableAssignments = new AvailableAssignmentsInfo(content);
                        return info;
                    }
				}
			}
		}
    }
}