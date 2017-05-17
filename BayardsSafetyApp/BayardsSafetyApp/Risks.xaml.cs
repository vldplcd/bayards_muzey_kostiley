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
        public Risks()
        {
            InitializeComponent();
            IsLoading = false;
            BackgroundColor = Color.FromHex("#efefef");
            Title = AppResources.LangResources.Risk;
            
        }

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
                        foreach (var r in _contents)
                        {
                            var rToDisp = r;
                            //var med = App.Database.MediaDatabase.GetItems<Media>().ToList().FindAll(m => m.Id_r == r.Id_r && 
                            //                                                                   m.Lang == AppResources.LangResources.Language).ToList().
                            //                                                                       Select(m => m.Url).ToList();
                            var med = Utils.DeserializeFromJson<List<Media>>((string)Application.Current.Properties["AllMedia"]).FindAll(m => m.Id_r == r.Id_r &&
                                                                                               m.Lang == AppResources.LangResources.Language).ToList();
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
            riskView.ItemsSource = _contents;
            riskView.SelectedItem = null;
        }
    }
}
