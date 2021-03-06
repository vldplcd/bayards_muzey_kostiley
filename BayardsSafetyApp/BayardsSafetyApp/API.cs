﻿using System;
using System.Collections.Generic;
using BayardsSafetyApp.Entities;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Globalization;
using BayardsSafetyApp.DTO;

namespace BayardsSafetyApp
{
    public class API
    {
        string host = (string)App.Current.Properties["host"]; //This is the host address. It should be equal to the one that is used by the company to run the server

        public string Host
        {
            get
            {
                return host;
            }
            set
            {
                host = value;
            }
        }
        //links to API methods
        const string UriGetAll = "{0}/api/getAll?apiKey={1}&lang={2}"; //main method. It gets all the information from the server that is used inside the app. (GET)
        const string UriUpdateTime = "{0}/api/getUpdateDate"; // gets the time when the server was updated (GET)
        const string UriCheckPassword = "{0}/api/checkPassword"; //checks the password encoded in md5 (POST)
        const string UriGetUsrAgr = "{0}/api/getUserAgreement?apiKey={1}&lang={2}";

        string UriImagePath = "{0}/ui/images/{1}"; //link to download image from database

        public string ImagePath //property for UriImagePath
        {
            get
            {
                return UriImagePath;
            }
            set
            {
                UriImagePath = value;
            }
        }

        string[] _langs = new string[] { "eng", "nl" };
        public string[] Langs
        {
            get { return _langs; }

            set
            {
                if (value != null || value.Length != 0)
                    _langs = value;
            }
        }

        /// <summary>
        /// Method that checks if it is needed to download data
        /// </summary>
        /// <returns>Returns true if connection Update is needed else returns false</returns>
        public async Task<bool> isUpdataNeeded(DateTime lastupdate)
        {
            DateTime current;
            using (HttpClient hc = new HttpClient())
            {
                var responseMsg = hc.GetAsync(string.Format(UriUpdateTime, Host)).Result;
                var resultStr = await responseMsg.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeAnonymousType(resultStr, new { Date = String.Empty });
                if (res == null || res.Date == null || !DateTime.TryParseExact(res.Date, "yyyy-MM-dd HH:mm:ss", new CultureInfo("fr-FR"), DateTimeStyles.None, out current))
                    throw new ArgumentException("Invalid returned date");
            }
            if (lastupdate < current)                
                return true;
            return false;
        }

        /// <summary>
        /// Method that sends password to the server
        /// </summary>
        /// <param name="password"></param>
        /// <returns>Returns true if password is correct, else returns false</returns>
        public async Task<bool> isPasswordCorrect(string password)
        {
            int? flag = 0;
            using (HttpClient hc = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                   { "password", password}
                };

                var content = new FormUrlEncodedContent(values);
                var response = hc.PostAsync(string.Format(UriCheckPassword, Host), content).Result;
                var responseString = await response.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeAnonymousType(responseString, new { check = new int?() });
                flag = res.check;
            }

            return flag == 1;

        }

        /// <summary>
        /// Method that gets all data
        /// </summary>
        /// <param name="risk"></param>
        /// <returns>List of SectionAPI type (DTO)</returns>
        public async Task<ShellRequest> GetAll(string lang)
        {
            var sections = new List<SectionAPI>();
            var data = new ShellRequest();
            int n = 0;
            while (n < 4)
            {
                try
                {
                    using (HttpClient hc = new HttpClient())
                    {
                        var responseMsg = await hc.GetAsync(string.Format(UriGetAll, Host, App.Current.Properties["password"], lang));
                        if(responseMsg.IsSuccessStatusCode)
                        {
                            var resultStr = await responseMsg.Content.ReadAsStringAsync();
                            if (resultStr.StartsWith("Error"))
                                throw new HttpRequestException(resultStr);
                            var res = JsonConvert.DeserializeObject<ShellRequest>(resultStr);
                            //var res = JsonConvert.DeserializeAnonymousType(resultStr,
                            //        new { Sections = new List<SectionAPI>() });
                            //sections.AddRange(res.Sections);
                            data = res;
                            if (data.Sections.Count == 0 || data.Sections[0].Id_s == null)
                                throw new Exception("No info downloaded. Trying to retry");
                        }
                        else
                        {
                            throw new HttpRequestException("The server has thrown an error code: " + responseMsg.StatusCode);
                        }
                    }
                    return data;
                }

                catch (Exception ex)
                {
                    n++;
                    if (n == 4)
                        throw ex;
                }
            }
            return data;
        }

        /// <summary>
        /// Method that gets checks internet connection
        /// </summary>
        /// <returns>Returns true if connection is set else returns false</returns>
        public bool CheckInternetConnection()
        {
            string CheckUrl = "http://google.com";

            try
            {
                using (HttpClient hc = new HttpClient())
                {
                    var responseMsg = hc.GetAsync(CheckUrl).Result;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetUserAgreement(string lang)
        {
            string agr;
            using (HttpClient hc = new HttpClient())
            {
                var responseMsg = hc.GetAsync(string.Format(UriGetUsrAgr, Host, App.Current.Properties["password"], lang)).Result;
                var resultStr = responseMsg.Content.ReadAsStringAsync().Result;
                var res = JsonConvert.DeserializeAnonymousType(resultStr, new { Content = "" });
                if (res == null || res.Content == null)
                    throw new ArgumentException("No user agreement");
                agr = res.Content;
            }
            return agr;
        }
    }
}
