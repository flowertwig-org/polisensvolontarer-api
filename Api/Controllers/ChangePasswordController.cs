using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using Api.Contracts;
using Api.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class ChangePasswordController : Controller
    {
        private AppSettings _appSettings;

        public ChangePasswordController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        [HttpPost]
        public IActionResult Post([FromForm]string currentPassword, [FromForm]string newPassword)
        {
            this.Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, post-check=0, pre-check=0");

            string step1Url = "";
            string step2Url = "";
            var step1InterestsValues = new List<KeyValuePair<string, string>>();
            var step2InterestsValues = new List<KeyValuePair<string, string>>();

            ChangePasswordResult info = new ChangePasswordResult();

            var isSecurePassword = false;

            var newPasswordStatus = LoginHelper.IsPasswordOk(newPassword);
            if (!newPasswordStatus)
            {
                info.IsWeakPassword = !newPasswordStatus;
                info.Warning = 1;
                return Json(info);
            }

            isSecurePassword = newPasswordStatus;

            // TODO: 1. Sanity checking
            // TODO: 2. Make login request

            var cookieContainer = new CookieContainer();

            byte[] cookieData;
            if (HttpContext.Session.TryGetValue("Session-Cookie", out cookieData))
            {
                var sessionCookie = System.Text.Encoding.UTF8.GetString(cookieData);
                cookieContainer.Add(new Uri("http://volontar.polisen.se/"), new Cookie("PHPSESSID", sessionCookie));

                using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                {
                    byte[] data;
                    if (this.HttpContext.Session.TryGetValue("MainNavigation", out data))
                    {
                        var content = System.Text.Encoding.UTF8.GetString(data);
                        var jArray = Newtonsoft.Json.JsonConvert.DeserializeObject(content) as Newtonsoft.Json.Linq.JObject;
                        var navigationInfo = jArray.ToObject<NavigationInfo>();
                        if (navigationInfo != null)
                        {
                            // TODO: 3. If no valid login, return false
                            // TODO: 4a. Add session info
                            // TODO: 4b. return true;
                            HttpClient client = new HttpClient(handler);
                            using (var response = client.GetAsync("http://volontar.polisen.se/" + navigationInfo.ChangePasswordUrl).Result)
                            {
                                using (var responseContent = response.Content)
                                {
                                    var step01Content = responseContent.ReadAsStringAsync().Result;

                                    // action="([^"]+)"
                                    var urlMatch = Regex.Match(step01Content, "action=\"(?<url>[^\"]+)\"");
                                    if (urlMatch.Success)
                                    {
                                        var urlGroup = urlMatch.Groups["url"];
                                        if (urlGroup.Success)
                                        {
                                            step1Url = "http://volontar.polisen.se/" + urlGroup.Value.Replace("../", "");
                                        }
                                    }

                                    // name="([^"]+)" value="([^"]+)"
                                    var nameAndValuePairsMatch = Regex.Matches(step01Content, "name=\"(?<name>[^\"]+)\" value=\"(?<value>[^\"]+)\"");
                                    foreach (Match nameAndValuePairMatch in nameAndValuePairsMatch)
                                    {
                                        if (nameAndValuePairMatch.Success)
                                        {
                                            var nameGroup = nameAndValuePairMatch.Groups["name"];
                                            var valueGroup = nameAndValuePairMatch.Groups["value"];
                                            if (nameGroup.Success && valueGroup.Success)
                                            {
                                                step1InterestsValues.Add(new KeyValuePair<string, string>(nameGroup.Value, valueGroup.Value));
                                            }
                                        }
                                    }

                                    if (step1InterestsValues.Count > 0)
                                    {
                                        step1InterestsValues.Add(new KeyValuePair<string, string>("password", currentPassword));
                                        step1InterestsValues.Add(new KeyValuePair<string, string>("submit_19_12", "Gå+vidare"));
                                    }
                                }
                            }

                            if (string.IsNullOrEmpty(step1Url) || step1InterestsValues.Count < 3)
                            {
                                info.Warning = 4;
                                return Json(info);
                            }

                            // Verify current password AND get values for step 2
                            var step1Content = new FormUrlEncodedContent(step1InterestsValues);
                            using (var response = client.PostAsync(step1Url, step1Content).Result)
                            {
                                using (var responseContent = response.Content)
                                {
                                    var step02Content = responseContent.ReadAsStringAsync().Result;

                                    // action="([^"]+)"
                                    var urlMatch = System.Text.RegularExpressions.Regex.Match(step02Content, "action=\"(?<url>[^\"]+)\"");
                                    if (urlMatch.Success)
                                    {
                                        var urlGroup = urlMatch.Groups["url"];
                                        if (urlGroup.Success)
                                        {
                                            step2Url = "http://volontar.polisen.se/" + urlGroup.Value.Replace("../", "");
                                        }
                                    }

                                    // name="([^"]+)" value="([^"]+)"
                                    var nameAndValuePairsMatch = System.Text.RegularExpressions.Regex.Matches(step02Content, "name=\"(?<name>[^\"]+)\" value=\"(?<value>[^\"]+)\"");
                                    foreach (Match nameAndValuePairMatch in nameAndValuePairsMatch)
                                    {
                                        if (nameAndValuePairMatch.Success)
                                        {
                                            var nameGroup = nameAndValuePairMatch.Groups["name"];
                                            var valueGroup = nameAndValuePairMatch.Groups["value"];
                                            if (nameGroup.Success && valueGroup.Success)
                                            {
                                                step2InterestsValues.Add(new KeyValuePair<string, string>(nameGroup.Value, valueGroup.Value));
                                            }
                                        }
                                    }

                                    if (step2InterestsValues.Count > 0)
                                    {
                                        step2InterestsValues.Add(new KeyValuePair<string, string>("cont_cont/password", newPassword));
                                        step2InterestsValues.Add(new KeyValuePair<string, string>("submit_28_2", "Byt+lösenord"));
                                    }
                                }
                            }

                            if (string.IsNullOrEmpty(step2Url) || step2InterestsValues.Count < 3)
                            {
                                info.Warning = 4;
                                return Json(info);
                            }

                            if (step1Url == step2Url)
                            {
                                // Invalid current password
                                info.Warning = 6;
                                return Json(info);
                            }

                            // Change password
                            var step2Content = new FormUrlEncodedContent(step2InterestsValues);
                            using (var response = client.PostAsync(step2Url, step2Content).Result)
                            {
                                using (var responseContent = response.Content)
                                {
                                    var step03Content = responseContent.ReadAsStringAsync().Result;

                                    // action="([^"]+)"
                                    var urlMatch = Regex.Match(step03Content, "Ditt l&ouml;senord &auml;r nu uppdaterat");
                                    if (urlMatch.Success)
                                    {
                                        info.IsSuccess = true;
                                        return Json(info);
                                    }
                                    else
                                    {
                                        info.Warning = 4;
                                        return Json(info);
                                    }
                                }
                            }
                        }
                        else
                        {
                            info.Warning = 3;
                            return Json(info);
                        }
                    }
                }
            }
            info.Warning = 3;
            return Json(info);
        }
    }
}