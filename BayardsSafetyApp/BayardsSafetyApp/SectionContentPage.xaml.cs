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
    public partial class SectionContentPage : TabbedPage //Section content page that has 2 tabs: risks and subsections. Presented when section/task is clicked
    {
        string _sId;
        public SectionContentPage(string sectionId, string sectionName)
        {
            InitializeComponent();
            BackgroundColor = Color.FromHex("#efefef");
            _sId = sectionId;
            Title = sectionName;

            TabbedPage_Appearing();
            var riskPage = new Risks() { ParentSection = sectionName };
            riskPage.Contents = Contents.Risks;
            Children.Add(riskPage);
            var subtPage = new Sections();
            subtPage.ParentSection = _sId;
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
        private void TabbedPage_Appearing() //on appearing retrieving data about task (all subtasks an risks)
        {
            bool flag = false;
            while (!flag)
            {
                try
                {
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
                catch (Exception)
                {

                }
            }
        }
    }
}
