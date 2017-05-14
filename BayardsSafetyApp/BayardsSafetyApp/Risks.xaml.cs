using BayardsSafetyApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BayardsSafetyApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Risks : ContentPage
    {
        List<RiskDetails> _risks;
        string _sId;
        public Risks(string sectionId, string sectionName)
        {
            InitializeComponent();
            _sId = sectionId;
            IsLoading = false;
            BackgroundColor = Color.FromHex("#efefef");
            Title = sectionName;
            
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

        bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        private void RiskButton_Clicked(object sender, SelectedItemChangedEventArgs e)
        {
            IsLoading = true;
            bool flag = false;
            IsLoading = true;
            while(!flag)
            {
                if (_risks == null)
                    _risks = new List<RiskDetails>();

                try
                {
                    if (_risks.Count == 0)
                    {
                        foreach (var r in _contents.Risks)
                        {
                            var rToDisp = r;
                            var med = App.Database.MediaDatabase.GetItems<Media>().ToList().FindAll(m => m.Id_r == r.Id_r && 
                                                                                               m.Lang == AppResources.LangResources.Language).ToList().
                                                                                                   Select(m => m.Url).ToList();
                            rToDisp.Media = med;
                            _risks.Add(new RiskDetails(rToDisp));
                        }
                    }
                    flag = true;
                    Navigation.PushAsync(new RisksCarousel(_risks, ((Risk)e.SelectedItem).Id_r, Title));
                }
                catch (Exception)
                {
                    //DisplayAlert("Error", ex.Message, "Ok");
                }
            }
            

        }

        private void Page_Appeared(object sender, EventArgs e)
        {
            //API api = new API();
            bool flag = false;
            while(!flag)
            {
                try
                {
                    //Contents = api.getSectionContent(_sId, AppResources.LangResources.Language).Result;
                    var d_risks = App.Database.RiskDatabase.GetItems<Risk>().ToList().FindAll(r => r.Parent_s == _sId
                                                                                        && r.Lang == AppResources.LangResources.Language).ToList();
                    if (d_risks != null)
                        Contents.Risks = d_risks.OrderBy(r => r.Name).ToList();
                        

                    var d_sects = App.Database.SectionDatabase.GetItems<Section>().ToList().FindAll(s => s.Parent_s == _sId
                                                                                        && s.Lang == AppResources.LangResources.Language).ToList();
                    if (d_sects != null)
                        Contents.Subsections = d_sects.OrderBy(r => r.Name).ToList();

                    sectView.ItemsSource = Contents.Subsections;
                    riskView.ItemsSource = Contents.Risks;

                    flag = true;
                }
                catch (Exception ex)
                {

                }
            }
            
        }

        private void sectView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            IsLoading = true;
            if (e.SelectedItem == null)
            {
                return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
            }
            Navigation.PushAsync(new Risks(((Section)e.SelectedItem).Id_s, ((Section)e.SelectedItem).Name));
        }
    }
}
