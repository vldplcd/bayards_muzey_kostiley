﻿using BayardsSafetyApp.DTO;
using BayardsSafetyApp.Entities;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BayardsSafetyApp.DBLoading
{
    public delegate void ProgressDelegate(double progr);
    public class LoadData
    {
        public ProgressDelegate OnProgressEvent;
        private API _api = new API();
        public double Process { get; set; }
        List<Risk> _risks;
        List<Section> _sections;
        List<Media> _mediaList;
        string[] _langs = new string[] { "eng", "nl" };
        //string databasePath = DependencyService.Get<ISQLite>().GetDatabasePath("bayards.db");

        private void UploadAll()
        {
            try
            {
                //using (var context = App.Database.SectionDatabase)
                //{
                //    if (_sections.Count != 0)
                //    {
                ////        context.DeleteAll<Section>();
                ////        context.InsertItems<Section>(_sections);
                //    }
                //}
                //using (var context = App.Database.RiskDatabase)
                //{
                //    if (_risks.Count != 0)
                //    {
                //        context.DeleteAll<Risk>();
                //        context.InsertItems<Risk>(_risks);
                //    }
                //}
                //using (var context = App.Database.MediaDatabase)
                //{
                //    if (_mediaList.Count != 0)
                //    {
                //        context.DeleteAll<Media>();
                //        context.InsertItems<Media>(_mediaList);
                //    }

                //}
                //var context = App.Database;
                //context.SectionDatabase.DeleteAll<Section>();
                //context.RiskDatabase.DeleteAll<Risk>();
                //context.MediaDatabase.DeleteAll<Media>();
                //context.SectionDatabase.InsertItems<Section>(_sections);
                //context.RiskDatabase.InsertItems<Risk>(_risks);
                //context.MediaDatabase.InsertItems<Media>(_mediaList);
                Application.Current.Properties["AllSections"] = Utils.SerializeToJson(_sections);
                Application.Current.Properties["AllRisks"] = Utils.SerializeToJson(_risks);
                Application.Current.Properties["AllMedia"] = Utils.SerializeToJson(_mediaList);
                try
                {
                    Application.Current.SavePropertiesAsync().Wait();
                }
                catch(Exception ex)
                {

                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public async Task ToDatabase()
        {
            Process = 0;
            OnProgressEvent?.Invoke(Process);
            _risks = new List<Risk>();
            _sections = new List<Section>();
            _mediaList = new List<Media>();
            var sectsAPI = new List<SectionAPI>();

            foreach(var lang in _langs)
            {
                sectsAPI = await _api.GetAll(lang);
                Process = 0.1 * (1/((double)_langs.Length));
                OnProgressEvent?.Invoke(Process);
                int n = sectsAPI.Count;
                foreach (var sAPI in sectsAPI)
                {
                    DecompSectionAPI(lang, "null", sAPI, ref _sections, ref _risks, ref _mediaList);
                    Process += (0.7 / (double)n) * (1 / ((double)_langs.Length));
                    OnProgressEvent?.Invoke(Process);
                }
                    
            }
            UploadAll();
                


        }

        public void DecompSectionAPI(string Lang, string ParentSect, SectionAPI sectAPI, ref List<Section> sects, ref List<Risk> risks, ref List<Media> mediaL)
        {
            sects.Add(new Section { Name = sectAPI.Name, Id_s = sectAPI.Id_s, Lang = Lang, Parent_s = ParentSect });

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
                        Media = r.Media
                    });
            }
            
            foreach(var r in risks)
            {

                foreach (var m in r.Media)
                    mediaL.Add(new Media { Lang = Lang, Id_r = r.Id_r, Url = m.Url, Type = m.Type });
            }
            var subsects = sectAPI.Subsections == null ? new SectionAPI[0] : sectAPI.Subsections;
            if (subsects.Length != 0)
            {
                foreach (var sAPI in sectAPI.Subsections)
                    DecompSectionAPI(Lang, sectAPI.Id_s, sAPI, ref sects, ref risks, ref mediaL);
            }
            

        }
        public async Task ToDatabaseCoplexAPI()
        {
            Process = 0;
            OnProgressEvent?.Invoke(Process);
            _risks = new List<Risk>();
            _sections = new List<Section>();
            _mediaList = new List<Media>();
            var sectIds = new List<string>();
            foreach (var lang in _langs)
            {
                var temp_s = _api.getCompleteSectionsList(lang).Result;
                if (temp_s.Count != 0)
                {
                    foreach (var s in temp_s)
                    {
                        if (lang == "eng")
                            sectIds.Add(s.Id_s);
                        s.Lang = lang;
                        s.Parent_s = "null";
                    }
                    _sections.AddRange(temp_s);
                }
            }
            Process = 0.1;
            OnProgressEvent?.Invoke(Process);
            if (sectIds.Count > 0)
            {
                var n = sectIds.Count;
                foreach (var sId in sectIds)
                {
                    AllSectionContent(sId).Wait();
                    Process += 1/(double)(n + 5);
                    OnProgressEvent?.Invoke(Process);
                }
                    
            }            
            UploadAll ();
        }

        private async Task AllSectionContent(string sectId)
        {
            var temp_sc = await _api.getSectionContent(sectId, "eng");
            var sectIds = new List<string>();
            var temp_risks = new List<Risk>();
            var temp_risk = new Risk();
            var temp_mediaList = new List<Media>();
            foreach (var r in temp_sc.Risks)
            {
                foreach (var lang in _langs)
                {
                    temp_risk = new Risk();
                    temp_risk = await GetRisk(r.Id_r, lang, sectId);
                    if (temp_risk != null)
                    {
                        temp_risks.Add(temp_risk);
                        foreach (var m in temp_risk.Media)
                            temp_mediaList.Add(new Media { Lang = lang, Url = m.Url, Id_r = r.Id_r });
                    }
                        
                }
            }
            _risks.AddRange(temp_risks);

            foreach (var lang in _langs)
            {
                var temp_subs = (await _api.getSectionContent(sectId, lang)).Subsections;
                if (temp_subs.Count != 0)
                {
                    foreach (var s in temp_subs)
                    {
                        if (lang == "eng")
                            sectIds.Add(s.Id_s);
                        s.Lang = lang;
                        s.Parent_s = sectId;
                    }
                    _sections.AddRange(temp_subs);
                }
            }
            if(sectIds.Count > 0)
            {
                foreach (var sId in sectIds)
                    AllSectionContent(sId).Wait();
            }


        }

        private async Task<Risk> GetRisk(string rId, string lang, string sectId)
        {
            
            var temp_risk = await _api.getRiskContent(rId, lang);
            if (temp_risk != null)
            {
                temp_risk.Lang = lang;
                temp_risk.Parent_s = sectId;
            }
            return temp_risk;
        }
    }
}
