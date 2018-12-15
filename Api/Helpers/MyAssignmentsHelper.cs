using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Api.Contracts;
using Microsoft.AspNetCore.Http;

namespace Api.Helpers
{
    public class MyAssignmentsHelper
    {
        public static MyAssignmentsInfo GetMyAssignmentsInfoFromUrl(HttpClientHandler handler, NavigationInfo navigation)
        {
            try
            {
                HttpClient client = new HttpClient(handler);

                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "VolontarPortal/1.0");

                using (var response = client.GetAsync("http://volontar.polisen.se/" + navigation.MyAssignmentsUrl).Result)
                {
                    using (var responseContent = response.Content)
                    {
                        var pageContent = responseContent.ReadAsStringAsync().Result;

                        var interestsIndex = pageContent.IndexOf("intresse", StringComparison.Ordinal);
                        var confirmsIndex = pageContent.IndexOf("Uttagen att medverka", StringComparison.Ordinal);
                        var reservationsIndex = pageContent.IndexOf("reservlista - meddelas", StringComparison.Ordinal);
                        var historyIndex = pageContent.IndexOf("Historik", StringComparison.Ordinal);

                        AvailableAssignmentsInfo interests = null;
                        AvailableAssignmentsInfo confirms = null;
                        AvailableAssignmentsInfo reservations = null;
                        AssignmentsHistoryInfo history = null;

                        if (interestsIndex != -1 && confirmsIndex != -1)
                        {
                            interests = new AvailableAssignmentsInfo(pageContent.Substring(interestsIndex, confirmsIndex - interestsIndex));
                        }
                        if (confirmsIndex != -1 && reservationsIndex != -1)
                        {
                            confirms = new AvailableAssignmentsInfo(pageContent.Substring(confirmsIndex, reservationsIndex - confirmsIndex));
                        }
                        if (reservationsIndex != -1)
                        {
                            reservations = new AvailableAssignmentsInfo(pageContent.Substring(reservationsIndex));
                        }
                        if (historyIndex != -1)
                        {
                            history = new AssignmentsHistoryInfo(pageContent.Substring(historyIndex));
                        }

                        var lastWeek = DateTime.Today.AddDays(-7);

                        return new MyAssignmentsInfo
                        {
                            IsLoggedIn = true,
                            Interests = interests != null ? interests.Assignments : new List<Assignment>(),
                            Confirms = confirms != null ? confirms.Assignments : new List<Assignment>(),
                            Reservations = reservations != null ? reservations.Assignments : new List<Assignment>(),
                            History = history != null ? history.Assignments.Where(a => a.Date >= lastWeek).OrderByDescending(a => a.Date).ToList() : new List<AssignmentHistory>()
                        };
                    }
                }
            }
            catch (System.Exception)
            {
                return new MyAssignmentsInfo { };
            }
        }

        public static MyAssignmentsInfo GetMyAssignments(HttpContext httpContext, CookieFailKeyInfo cookieFailKeyInfo)
        {
            string key = "";
            if (cookieFailKeyInfo.IsVaild)
            {
                key = cookieFailKeyInfo.Key;
                var tmpUrl = cookieFailKeyInfo.AvailableAssignmentsUrl;
            }
            else
            {
                byte[] cookieData;
                if (httpContext.Session.TryGetValue("Session-Cookie", out cookieData))
                {
                    key = System.Text.Encoding.UTF8.GetString(cookieData);
                }
            }

            var navigationInfo = NavigationHelper.GetNavigation(httpContext, cookieFailKeyInfo);
            if (navigationInfo == null)
            {
                return new MyAssignmentsInfo { IsLoggedIn = false };
            }

            var cookieContainer = new CookieContainer();

            cookieContainer.Add(new Uri("http://volontar.polisen.se/"), new Cookie("PHPSESSID", key));

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                return GetMyAssignmentsInfoFromUrl(handler, navigationInfo);
            }
        }
    }
}