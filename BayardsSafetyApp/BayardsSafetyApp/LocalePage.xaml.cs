﻿using System;
using System.Globalization;
using BayardsSafetyApp.AppReses;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BayardsSafetyApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocalePage : ContentPage
    {
        public LocalePage()
        {
            InitializeComponent();
            BackgroundColor = Color.FromHex("#efefef");
            LocaleLabel.Text = LangResources.LocaleLabel;
            EnButton.Text = LangResources.EnButton;
            NlButton.Text = LangResources.NlButton;
        }
        protected override Boolean OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            return true;
        }

        private void EnButton_Clicked(object sender, EventArgs e)
        {
            LangResources.Culture = new CultureInfo("en");
            Navigation.PushAsync(new UserAgreementPage());
        }

        private void NlButton_Clicked(object sender, EventArgs e)
        {
            LangResources.Culture = new CultureInfo("nl");
            Navigation.PushAsync(new UserAgreementPage());
        }
    }
}
