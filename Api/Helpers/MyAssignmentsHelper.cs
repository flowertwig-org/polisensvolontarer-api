using System;
using System.Collections.Generic;
using System.Net.Http;
using Api.Contracts;

namespace Api.Helpers
{
    public class MyAssignmentsHelper
    {
        public static MyAssignmentsInfo GetMyAssignmentsInfoFromUrl(HttpClientHandler handler, NavigationInfo navigation)
        {
            try
            {
                HttpClient client = new HttpClient(handler);
                using (var response = client.GetAsync("http://volontar.polisen.se/" + navigation.MyAssignmentsUrl).Result)
                {
                    using (var responseContent = response.Content)
                    {
                        var pageContent = responseContent.ReadAsStringAsync().Result;

                        var interestsIndex = pageContent.IndexOf("intresse", StringComparison.Ordinal);
                        var confirmsIndex = pageContent.IndexOf("Uttagen att medverka", StringComparison.Ordinal);
                        var reservationsIndex = pageContent.IndexOf("reservlista - meddelas", StringComparison.Ordinal);

                        AvailableAssignmentsInfo interests = null;
                        AvailableAssignmentsInfo confirms = null;
                        AvailableAssignmentsInfo reservations = null;

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

                        return new MyAssignmentsInfo
                        {
                            Interests = interests.Assignments,
                            Confirms = confirms.Assignments,
                            Reservations = reservations.Assignments
                        };
                    }
                }
            }
            catch (System.Exception ex)
            {
                return new MyAssignmentsInfo { Reservations = new List<Assignment> { new AssignmentDetail { Description = "Unknown Error: " + ex } } };
            }
        }
    }
}