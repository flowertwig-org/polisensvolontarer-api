﻿using System.Linq;
using System.Net;
using System.Net.Http;
using Api.Contracts;
using Api.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private AppSettings _appSettings;
        public LoginController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        // GET api/values
        [HttpGet]
        public bool Get()
        {
            this.Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, post-check=0, pre-check=0");

            try
            {
                byte[] data;
                if (HttpContext.Session.TryGetValue("Session-Cookie", out data)) {
                    string content = System.Text.Encoding.UTF8.GetString(data);
                    return (!string.IsNullOrEmpty(content));
                }
                // TODO: 1. Sanity checking
                // TODO: 2. Return login status
                // TODO: 3. If no valid login, return false
                // TODO: 4a. Add session info
                // TODO: 4b. return true;

                return false;
            }
            catch (System.Exception)
            {
                return false;
            }

        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromForm]string username, [FromForm]string password, [FromForm]string page, [FromForm]string query)
        {
            this.Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, post-check=0, pre-check=0");

            var successfullLogin = false;
            string redirectUrl = null;
            var isSecurePassword = false;

            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                var passwordStatus = LoginHelper.IsPasswordOk(password);
                var passwordStatusQuery = "";

                if (!passwordStatus) {
                    passwordStatusQuery = "?warning=1";
                }

                isSecurePassword = passwordStatus;

                // TODO: 1. Sanity checking
                // TODO: 2. Make login request
                var loginUrl = LoginHelper.GetLoginUrl(handler);
                var info = LoginHelper.Login(handler, loginUrl, username, password);

                successfullLogin = info.Status;

                if (info.Status)
                {
                    var cookies = cookieContainer.GetCookies(new System.Uri("http://volontar.polisen.se"));
                    foreach (Cookie cookie in cookies)
                    {
                        if (cookie.Name == "PHPSESSID")
                        {
                            HttpContext.Session.Set("Session-Cookie", System.Text.Encoding.UTF8.GetBytes(cookie.Value));
                        }
                    }

                    var availableAssignments = Newtonsoft.Json.JsonConvert.SerializeObject(info.AvailableAssignments.Assignments.ToArray());
                    HttpContext.Session.Set("AvailableAssignments", System.Text.Encoding.UTF8.GetBytes(availableAssignments));


                    var mainNavigation = Newtonsoft.Json.JsonConvert.SerializeObject(info.MainNavigation);
                    HttpContext.Session.Set("MainNavigation", System.Text.Encoding.UTF8.GetBytes(mainNavigation));

                    switch (page)
                    {
                        case "assignment":
                            if (query != null && query.IndexOf('?') == 0) {
                                // lookup assignment
                                var assignmentId = query.Replace("?key=", "");
                                var assignment = info.AvailableAssignments.Assignments.FirstOrDefault(a => a.Id == assignmentId);
                                if (assignment == null)
                                {
                                    // Assignment can't be found, go to start page instead
                                    redirectUrl = _appSettings.WebSiteUrl + "/restricted/" + passwordStatusQuery;
                                    //return this.Redirect(_appSettings.WebSiteUrl + "/restricted/" + passwordStatusQuery);
                                }
                                else
                                {
                                    // go to assignment
                                    redirectUrl = _appSettings.WebSiteUrl + "/restricted/assignment/?key=" + assignmentId + passwordStatusQuery.Replace('?', '&');
                                    //return this.Redirect(_appSettings.WebSiteUrl + "/restricted/assignment/?key=" + assignmentId + passwordStatusQuery.Replace('?', '&'));
                                }
                            }
                            else {
                                redirectUrl = _appSettings.WebSiteUrl + "/restricted/available-assignments/" + passwordStatusQuery;
                                //return this.Redirect(_appSettings.WebSiteUrl + "/restricted/available-assignments/" + passwordStatusQuery);
                            }
                            break;
                        case "available-assignments":
                            redirectUrl = _appSettings.WebSiteUrl + "/restricted/available-assignments/" + passwordStatusQuery;
                            //return this.Redirect(_appSettings.WebSiteUrl + "/restricted/available-assignments/" + passwordStatusQuery);
                            break;
                        default:
                            redirectUrl = _appSettings.WebSiteUrl + "/restricted/" + passwordStatusQuery;
                            //return this.Redirect(_appSettings.WebSiteUrl + "/restricted/" + passwordStatusQuery);
                            break;
                    }
                }
                else {
                    redirectUrl = _appSettings.WebSiteUrl + "/?warning=2";
                    //return this.Redirect(_appSettings.WebSiteUrl + "/?warning=2");
                }

                // TODO: 3. If no valid login, return false
                // TODO: 4a. Add session info
                // TODO: 4b. return true;
            }

            return Json(new LoginResult
            {
                IsSuccess = successfullLogin,
                IsWeakPassword = !isSecurePassword,
                RedirectUrl = redirectUrl
            });
        }
    }
}