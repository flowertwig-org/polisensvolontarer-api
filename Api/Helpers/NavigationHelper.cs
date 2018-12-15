using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Api.Contracts;
using Microsoft.AspNetCore.Http;

namespace Api.Helpers
{
    public class NavigationHelper
    {
        public static NavigationInfo GetNavigation(HttpContext httpContext, CookieFailKeyInfo cookieFailKeyInfo)
        {
            if (!cookieFailKeyInfo.IsVaild)
            {
                byte[] cookieData;
                if (httpContext.Session.TryGetValue("MainNavigation", out cookieData))
                {
                    var content = System.Text.Encoding.UTF8.GetString(cookieData);
                    var jArray = Newtonsoft.Json.JsonConvert.DeserializeObject(content) as Newtonsoft.Json.Linq.JObject;
                    var navigationInfo = jArray.ToObject<NavigationInfo>();
                    if (navigationInfo != null)
                    {
                        return navigationInfo;
                    }
                }
                return null;
            }
            else
            {
                var cookieContainer = new CookieContainer();
                cookieContainer.Add(new Uri("http://volontar.polisen.se/"), new Cookie("PHPSESSID", cookieFailKeyInfo.Key));
                using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                {
                    HttpClient client = new HttpClient(handler);
                    using (var response = client.GetAsync("http://volontar.polisen.se/" + cookieFailKeyInfo.AvailableAssignmentsUrl).Result)
                    {
                        using (var responseContent = response.Content)
                        {
                            var content = responseContent.ReadAsStringAsync().Result;
                            if (!LoginHelper.IsLoginContent(content))
                            {
                                return null;
                            }
                            else
                            {
                                return new NavigationInfo(content);
                            }
                        }
                    }
                }
            }
        }
    }
}