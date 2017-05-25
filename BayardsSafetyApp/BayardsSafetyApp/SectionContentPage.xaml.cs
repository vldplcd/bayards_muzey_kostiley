using BayardsSafetyApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace BayardsSafetyApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SectionContentPage : TabbedPage
    {
        string _sId;
        public SectionContentPage(string sectionId, string sectionName)
        {
            InitializeComponent();
            BackgroundColor = Color.FromHex("#efefef");
            _sId = sectionId;
            Title = sectionName;

            TabbedPage_Appearing();
            var riskPage = new Risks();
            riskPage.Contents = Contents.Risks;
            Children.Add(riskPage);
            var subtPage = new Sections();
            subtPage.Contents = Contents.Subsections;
            subtPage.ToolbarItems.Clear();
            subtPage.Title = AppReses.LangResources.Task;
            Children.Add(subtPage);
            
            
        }
        SectionContents _contents = new SectionContents();
        public SectionContents Contents
        {
            get { return _contents; }
            set
            {
                _contents = value;
                OnPropertyChanged();
            }
        }
        private void TabbedPage_Appearing()
        {
            bool flag = false;
            while (!flag)
            {
                try
                {
                    //Contents = api.getSectionContent(_sId, AppResources.LangResources.Language).Result;
                    //var d_risks = App.Database.RiskDatabase.GetItems<Risk>().ToList().FindAll(r => r.Parent_s == _sId
                    //                                                                    && r.Lang == AppResources.LangResources.Language).ToList();
                    var d_risks = Utils.DeserializeFromJson<List<Risk>>((string)Application.Current.Properties["AllRisks"]).FindAll(r => r.Parent_s == _sId
                                                                                        && r.Lang == AppReses.LangResources.Language).ToList();
                    if (d_risks != null)
                        Contents.Risks = d_risks.OrderBy(r => r.Order).ThenBy(r => r.Name).ToList();


                    var d_sects = Utils.DeserializeFromJson<List<Section>>((string)Application.Current.Properties["AllSections"]).FindAll(s => s.Parent_s == _sId
                                                                                        && s.Lang == AppReses.LangResources.Language).ToList();
                    if (d_sects != null)
                        Contents.Subsections = d_sects.OrderBy(s => s.Order).ThenBy(s => s.Name).ToList();
                    
                    
                    flag = true;
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
