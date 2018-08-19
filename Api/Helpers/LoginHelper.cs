﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Api.Contracts;

namespace Api.Helpers
{
    public class LoginHelper
    {

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

        public static bool IsPasswordOk(string password) {
            try
            {
                var sha = System.Security.Cryptography.SHA1.Create();
                var data = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                var hash = string.Join("", data.Select(b => b.ToString("x2")).ToArray()).ToUpper();
                var hashPrefix = hash.Substring(0, 5);

                HttpClient httpClient = new HttpClient();
                httpClient.Timeout = new System.TimeSpan(0, 0, 1);
                var result = httpClient.GetStringAsync("https://api.pwnedpasswords.com/range/" + hashPrefix).Result;

                if (!string.IsNullOrEmpty(result) && result.Contains(hash.Substring(5))) {
                    return false;
                }
            }
            catch (System.Exception)
            {
                // Ignore any possible error that might occure, this is a nice to have feature
                return true;
            }
            return true;
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