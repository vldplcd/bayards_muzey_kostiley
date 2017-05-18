using System;
using System.Collections.Generic;
using BayardsSafetyApp.Entities;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Globalization;
using BayardsSafetyApp.DTO;

namespace BayardsSafetyApp
{
    //TODO: ОБЪЕДИНИТЬ В ОДИН МЕТОД ЗАПРОС
    public class API
    {
        string host = "http://vhost29450.cpsite.ru";

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
        const string UriGetAll = "{0}/api/getAll?lang={1}";
        const string UriSectionsListTemplate = "{0}/api/allSections?lang={1}";
        const string UriSectionContent = "{0}/api/section?sectionid={1}&lang={2}";
        const string UriRiskContent = "{0}/api/risk?riskid={1}&lang={2}";
        const string UriUpdateTime = "{0}/api/getUpdateDate";

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

        //TODO: 
        //Добавить поисковые запросы в функционал API

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
            using (var context = App.Database)
            {
                //var a = context.RiskDatabase.GetItems<Risk>();
                //if (context.RiskDatabase.IsEmpty<Risk>() || context.SectionDatabase.IsEmpty<Section>())
                //    return true;
            }
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
        public bool isPasswordCorrect(string password)
        {
            if (password == "central") return true; //УДАЛИТЬ ПОСКОРЕЕЕЕЕ
            using (HttpClient hc = new HttpClient())
            {
                //POST REQUEST
            }
            return false;

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
                        res.Risk.Media = res.Media;
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
                        var responseMsg = await hc.GetAsync(string.Format(UriGetAll, Host, lang));
                        if(responseMsg.IsSuccessStatusCode)
                        {
                            var resultStr = await responseMsg.Content.ReadAsStringAsync();
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
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
