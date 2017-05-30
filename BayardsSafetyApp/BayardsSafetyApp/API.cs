using System;
using System.Linq;
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
        string host = "http://vhost29450.cpsite.ru"; //This is the host address. It should be equal to the one that is used by the company to run the server

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
        const string UriSectionsListTemplate = "{0}/api/allSections?lang={1}"; // gets all the main sections (GET)
        const string UriSectionContent = "{0}/api/section?sectionid={1}&lang={2}"; // gets section content (GET)
        const string UriRiskContent = "{0}/api/risk?riskid={1}&lang={2}"; // gets risk content (GET)
        const string UriUpdateTime = "{0}/api/getUpdateDate"; // gets the time when the server was updated (GET)
        const string UriCheckPassword = "{0}/api/checkPassword"; //checks the password encoded in md5 (POST)

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
        /// Method that gets the complete list of sections; language is specified with language variable
        /// </summary>
        /// <returns>List of sections</returns>
        public async Task<List<Section>> getCompleteSectionsList(string language)
        {
            string requestUri = String.Format(UriSectionsListTemplate, Host, language);
            List<Section> result;
            int n = 0;
            while (n < 4)
            {
                try
                {

                    using (HttpClient hc = new HttpClient() { Timeout = new TimeSpan(0, 0, 10) })
                    {
                        var responseMsg = hc.GetAsync(requestUri).Result;
                        var resultStr = responseMsg.Content.ReadAsStringAsync().Result;
                        var res = JsonConvert.DeserializeAnonymousType(resultStr, new { Sections = new List<Section>() });
                        //result = JsonConvert.DeserializeObject<ShellRequest<Section>>(resultStr).Data;
                        result = res.Sections;
                        if (result.Count == 0 || result[0].Id_s == null)
                            throw new Exception("No info downloaded.");
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    n++;
                    if (n == 4)
                        throw ex;
                }
            }
            return null;

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
        /// Method that gets the list of all risks and subsections from specified section by id and language
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns> 
        public async Task<SectionContents> getSectionContent(string Id, string language)
        {
            SectionContents result;
            string requestUri = String.Format(UriSectionContent, Host, Id, language);
            int n = 0;
            while (n < 4)
            {
                try
                {
                    using (HttpClient hc = new HttpClient())
                    {
                        var responseMsg = hc.GetAsync(requestUri).Result;
                        var resultStr = responseMsg.Content.ReadAsStringAsync().Result;
                        result = JsonConvert.DeserializeObject<SectionContents>(resultStr);
                        if (result.Section == null)
                            throw new Exception("No info downloaded. Trying to retry");
                    }
                    return result;
                }

                catch (Exception ex)
                {
                    n++;
                    if (n == 4)
                        throw ex;
                }
            }
            return null;
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
        /// Method that gets all data to the specified risk by id and lang
        /// </summary>
        /// <param name="risk"></param>
        /// <returns>List of all links with risk data</returns>
        public async Task<Risk> getRiskContent(string Id, string language)
        {
            Risk result;
            string requestUri = string.Format(UriRiskContent, Host, Id, language);
            int n = 0;
            while (n < 4)
            {
                try
                {
                    using (HttpClient hc = new HttpClient())
                    {
                        var responseMsg = hc.GetAsync(requestUri).Result;
                        var resultStr = responseMsg.Content.ReadAsStringAsync().Result;
                        var res = JsonConvert.DeserializeAnonymousType(resultStr, new { Risk = new Risk(), Media = new List<Media>() });
                        var temp_media = new List<Media>();
                        foreach (var m in res.Media)
                            temp_media.Add(new Media { Id_r = m.Id_r, Lang = m.Lang, Type = m.Type, Url = string.Format(UriImagePath, Host, m.Url) });
                        res.Risk.Media = temp_media;
                        result = res.Risk;
                        if (result.Id_r == null)
                            throw new Exception("No info downloaded. Trying to retry");
                    }
                    return result;
                }

                catch (Exception ex)
                {
                    n++;
                    if (n == 4)
                        throw ex;
                }
            }
            return null;
        }
        /// <summary>
        /// Method that gets all data
        /// </summary>
        /// <param name="risk"></param>
        /// <returns>List of SectionAPI type (DTO)</returns>
        public async Task<List<SectionAPI>> GetAll(string lang)
        {
            var sections = new List<SectionAPI>();
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
                            var res = JsonConvert.DeserializeAnonymousType(resultStr,
                                    new { Sections = new List<SectionAPI>() });
                            sections.AddRange(res.Sections);
                            if (sections.Count == 0 || sections[0].Id_s == null)
                                throw new Exception("No info downloaded. Trying to retry");
                        }
                        else
                        {
                            throw new HttpRequestException("The server has thrown an error code: " + responseMsg.StatusCode);
                        }
                    }
                    return sections;
                }

                catch (Exception ex)
                {
                    n++;
                    if (n == 4)
                        throw ex;
                }
            }
            return sections;
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
    }
}
