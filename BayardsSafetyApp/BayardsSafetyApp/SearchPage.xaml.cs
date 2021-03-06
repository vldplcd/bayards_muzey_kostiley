﻿using BayardsSafetyApp.Entities;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BayardsSafetyApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchPage : ContentPage //Search page
    {
        public SearchPage(Sections foundPage)
        {
            InitializeComponent();
            SearchCommand = new Command(() => { SearchCommandExecute(); });
            found = foundPage;
            risksLabel.Text = AppReses.LangResources.Risk;
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
        //List<Section> foundSections;
        private void SearchCommandExecute()
        {
            foundRisks = Utils.DeserializeFromJson<List<Risk>>((string)Application.Current.Properties["AllRisks"]).FindAll(i => i.Name.ToLower().Contains(SearchedText.ToLower()));
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

        private void RiskButton_Clicked(object sender, SelectedItemChangedEventArgs e) //Navigate to clicked risk
        {
            if (e.SelectedItem == null)
            {
                return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
            }
            var rToDisp = (Risk)e.SelectedItem;
            found.Found = new RiskDetails(rToDisp);
            Device.BeginInvokeOnMainThread(() => {
                Navigation.PopModalAsync();
            });
        }

        private void searchcustomer_TextChanged(object sender, TextChangedEventArgs e) //search method that is executed when text is changed
        {
            if (e.NewTextValue == null)
            {
                foundRisks = Utils.DeserializeFromJson<List<Risk>>((string)Application.Current.Properties["AllRisks"]).FindAll(r => r.Lang == AppReses.LangResources.Language);
                riskView.ItemsSource = foundRisks;
            }
            else
            {
                foundRisks = Utils.DeserializeFromJson<List<Risk>>((string)Application.Current.Properties["AllRisks"]).FindAll(r => r.Lang == AppReses.LangResources.Language);
                foundRisks = foundRisks.FindAll(i => i.Name.ToLower().Contains(e.NewTextValue.ToLower()));
                riskView.ItemsSource = foundRisks;
            }
            
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            foundRisks = Utils.DeserializeFromJson<List<Risk>>((string)Application.Current.Properties["AllRisks"]).FindAll(r => r.Lang == AppReses.LangResources.Language);
            riskView.ItemsSource = foundRisks;
        }

        private void Back_Button_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() => {
                Navigation.PopModalAsync();
            });
        }
    }
}
