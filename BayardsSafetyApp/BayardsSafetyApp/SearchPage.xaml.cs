using BayardsSafetyApp.Entities;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BayardsSafetyApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchPage : ContentPage
    {
        public SearchPage(Sections foundPage)
        {
            InitializeComponent();
            SearchCommand = new Command(() => { SearchCommandExecute(); });
            found = foundPage;
        }
        Sections found;
        public ICommand SearchCommand { get; set; }
        private string _searchedText;
        public string SearchedText
        {
            get { return _searchedText; }
            set { _searchedText = value; SearchCommandExecute(); }
        }
        List<Risk> foundRisks;
        List<Section> foundSections;
        private void SearchCommandExecute()
        {
            foundRisks = Utils.DeserializeFromJson<List<Risk>>((string)Application.Current.Properties["AllRisks"]).FindAll(i => i.Name.ToLower().Contains(SearchedText.ToLower()));
            foundSections = Utils.DeserializeFromJson<List<Section>>((string)Application.Current.Properties["AllSections"]).FindAll(i => i.Name.ToLower().Contains(SearchedText.ToLower()));
            sectView.ItemsSource = foundSections;
            riskView.ItemsSource = foundRisks;
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
            if (e.SelectedItem == null)
            {
                return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
            }
            API api = new API();
            var rToDisp = api.getRiskContent(((Risk)e.SelectedItem).Id_r, AppResources.LangResources.Language).Result;
            found.Found = new RiskDetails(rToDisp);
            Device.BeginInvokeOnMainThread(() => {
                Navigation.PopModalAsync();
                //Navigation.PushAsync(new Risks(((Section)e.SelectedItem).Id_s, ((Section)e.SelectedItem).Name));
            });
        }
        private void sectView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            IsLoading = true;
            if (e.SelectedItem == null)
            {
                return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
            }
            found.Found = new Risks(((Section)e.SelectedItem).Id_s, ((Section)e.SelectedItem).Name);
            Device.BeginInvokeOnMainThread(() => {
                Navigation.PopModalAsync();
            });
            
        }

        private void searchcustomer_TextChanged(object sender, TextChangedEventArgs e)
        {

            foundRisks = Utils.DeserializeFromJson<List<Risk>>((string)Application.Current.Properties["AllRisks"]).FindAll(r => r.Lang == AppResources.LangResources.Language);
            foundRisks = foundRisks.FindAll(i => i.Name.ToLower().Contains(e.NewTextValue.ToLower()));
            foundSections = Utils.DeserializeFromJson<List<Section>>((string)Application.Current.Properties["AllSections"]).FindAll(s => s.Lang == AppResources.LangResources.Language);
            foundSections = foundSections.FindAll(i => i.Name.ToLower().Contains(e.NewTextValue.ToLower()));
            sectView.ItemsSource = foundSections;
            riskView.ItemsSource = foundRisks;
        }
    }
}
