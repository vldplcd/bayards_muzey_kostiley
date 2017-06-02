using BayardsSafetyApp.DTO;
using BayardsSafetyApp.Entities;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BayardsSafetyApp.DBLoading
{
    public delegate void ProgressDelegate(double progr);
    public class LoadData //Class that loads data from server (using API)
    {
        public ProgressDelegate OnProgressEvent;
        private API _api = new API();
        public double Process { get; set; }
        List<Risk> _risks;
        List<Section> _sections;
        List<Media> _mediaList;
        List<Location> _locations;
        string[] _langs = new string[] { "eng", "nl" };

        private void UploadAll() //method to save loaded data
        {
            try
            {
                Application.Current.Properties["AllSections"] = Utils.SerializeToJson(_sections);
                Application.Current.Properties["AllRisks"] = Utils.SerializeToJson(_risks);
                Application.Current.Properties["AllMedia"] = Utils.SerializeToJson(_mediaList);
                Application.Current.Properties["AllLocations"] = Utils.SerializeToJson(_locations);
                Application.Current.SavePropertiesAsync().Wait();


            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public async Task ToDatabase() //method to load all data from server
        {
            try
            {
                if (Application.Current.Properties.ContainsKey("AllMedia")) //delete media if exists
                {
                    var mediaToDelete = Utils.DeserializeFromJson<List<Media>>((string)Application.Current.Properties["AllMedia"]).FindAll(m => m.Type == "image");
                    foreach (var media in mediaToDelete)
                        await FileSystem.Current.GetFileFromPathAsync(media.Url).Result.DeleteAsync();
                }
            }
            catch { }
            Process = 0;
            OnProgressEvent?.Invoke(Process);
            _risks = new List<Risk>();
            _sections = new List<Section>();
            _mediaList = new List<Media>();
            _locations = new List<Location>();
            var sectsAPI = new List<SectionAPI>();
            List<Location> temp_locs;
            foreach(var lang in _langs)
            {
                temp_locs = (await _api.GetAll(lang)).Locations;
                foreach (var loc in temp_locs)
                {
                    _locations.Add(new Location
                    {
                        Name = loc.Name,
                        Content = loc.Content,
                        Lang = lang,
                        Latitude = loc.Latitude,
                        Longitude = loc.Longitude,
                        Id_l = loc.Id_l
                    });
                }
                sectsAPI = (await _api.GetAll(lang)).Sections;
                Process = 0.1 * (1/((double)_langs.Length));
                OnProgressEvent?.Invoke(Process);
                int n = sectsAPI.Count;
                foreach (var sAPI in sectsAPI)
                {
                    if(!sAPI.Name.StartsWith("service"))
                    {
                        DecompSectionAPI(lang, "null", sAPI, ref _sections, ref _risks, ref _mediaList);
                        Process += (0.7 / (double)n) * (1 / ((double)_langs.Length));
                        OnProgressEvent?.Invoke(Process);
                    }
                    
                }
                    
            }
            UploadAll();
                


        }

        public void DecompSectionAPI(string Lang, string ParentSect, SectionAPI sectAPI, ref List<Section> sects, ref List<Risk> risks, ref List<Media> mediaL) //retrieving data from SectionAPI to Section, Risk, Media (recursive) 
        {
            sects.Add(new Section { Name = sectAPI.Name, Id_s = sectAPI.Id_s, Lang = Lang, Parent_s = ParentSect, Order = sectAPI.Order });

            var temprisks = sectAPI.Risks == null ? new Risk[0] : sectAPI.Risks;
            if(temprisks.Length != 0)
            {
                foreach (var r in sectAPI.Risks)
                    risks.Add(new Risk
                    {
                        Id_r = r.Id_r,
                        Lang = Lang,
                        Name = r.Name,
                        Content = r.Content,
                        Parent_s = sectAPI.Id_s,
                        Media = r.Media,
                        Order = r.Order
                    });
            }
            
            foreach(var r in temprisks)
            {

                foreach (var m in r.Media)
                {
                    string UrlToSave;
                    if (m.Type == "image")
                        UrlToSave = SaveImage(m.Url).Result;
                    else
                        UrlToSave = m.Url;
                    

                    mediaL.Add(new Media { Lang = Lang, Id_r = r.Id_r, Url = UrlToSave, Type = m.Type, Text = m.Text });                    
                }
                    
            }
            var subsects = sectAPI.Subsections == null ? new SectionAPI[0] : sectAPI.Subsections;
            if (subsects.Length != 0)
            {
                foreach (var sAPI in sectAPI.Subsections)
                    DecompSectionAPI(Lang, sectAPI.Id_s, sAPI, ref sects, ref risks, ref mediaL);
            }
            

        }

        private async Task<string> SaveImage(string fileName) //downloading images to device
        {
            string filePath;
            try
            {
                if (await FileSystem.Current.LocalStorage.CheckExistsAsync(fileName) != ExistenceCheckResult.FileExists)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        using (HttpResponseMessage response = await client.GetAsync(string.Format(_api.ImagePath, _api.Host, fileName), HttpCompletionOption.ResponseHeadersRead))
                        {
                            IFile file = await FileSystem.Current.LocalStorage.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                            filePath = file.Path;
                            using (Stream stream = await file.OpenAsync(FileAccess.ReadAndWrite))
                            {
                                var buffer = await response.Content.ReadAsByteArrayAsync();
                                await stream.WriteAsync(buffer, 0, buffer.Length);
                            }
                        }
                    }
                }
                else
                    filePath = (await FileSystem.Current.LocalStorage.GetFileAsync(fileName)).Path;
                return filePath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
    }
}
