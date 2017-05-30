using BayardsSafetyApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BayardsSafetyApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Risks : ContentPage //Risks page that is presented in section content (left tab)
    {
        List<RiskDetails> _risks;

        public Risks()
        {
            InitializeComponent();
            IsLoading = false;
            BackgroundColor = Color.FromHex("#efefef");
            Title = AppReses.LangResources.Risk;
            
        }
        public string ParentSection { get; set; }
        List<Risk> _contents = new List<Risk>();
        public List<Risk> Contents
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

        private void RiskButton_Clicked(object sender, SelectedItemChangedEventArgs e) //Navigating to clicked risk details page that will be in risk carousel page
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
                        foreach (var r in _contents)
                        {
                            var rToDisp = r;
                            var med = Utils.DeserializeFromJson<List<Media>>((string)Application.Current.Properties["AllMedia"]).FindAll(m => m.Id_r == r.Id_r &&
                                                                                               m.Lang == AppReses.LangResources.Language).ToList();
                            rToDisp.Media = med;
                            _risks.Add(new RiskDetails(rToDisp));
                        }
                    }
                    flag = true;
                    Navigation.PushAsync(new RisksCarousel(_risks, ((Risk)e.SelectedItem).Id_r, ParentSection));
                }
                catch (Exception)
                {
                }
            }
            

        }

        private void Page_Appeared(object sender, EventArgs e)
        {
            riskView.ItemsSource = _contents;
            riskView.SelectedItem = null;
        }
    }
}
