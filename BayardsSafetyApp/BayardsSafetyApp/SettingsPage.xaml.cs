using BayardsSafetyApp.AppReses;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BayardsSafetyApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        Dictionary<string, string> langsToPicker = new Dictionary<string, string>
        {
            {"Dutch",  "nl"}, {"English", "en"}
        };
        public SettingsPage()
        {
            InitializeComponent();
            langPicker.Title = "Choose language";
            langPicker.ItemsSource = langsToPicker.Keys.ToList();
            updMessage.Text = $"Last update: {((DateTime)Application.Current.Properties["UpdateTime"]).ToString()}.";
        }

        private async void checkUpdater_button_Clicked(object sender, EventArgs e)
        {
            AInd.IsEnabled = true;
            AInd.IsRunning = true;
            checkUpdater_button.IsEnabled = false;
            API api = new API();
            if (api.CheckInternetConnection())
            {
                switch ((string)checkUpdater_button.CommandParameter)
                {
                    case "check":
                        if(api.isUpdataNeeded(((DateTime)Application.Current.Properties["UpdateTime"])).Result)
                        {
                            checkUpdater_button.Text = "Update";
                            checkUpdater_button.CommandParameter = "update";
                            updMessage.Text = "You can update the information";
                        }
                        else
                        {
                            updMessage.Text = $"Last update: {((DateTime)Application.Current.Properties["UpdateTime"]).ToString()}. No new updates found";
                        }
                        break;
                    case "update":
                        if(api.isPasswordCorrect((string)Application.Current.Properties["password"]).Result)
                        {
                            if (await DisplayAlert("Warning", LangResources.DownloadWarn, "OK", "Cancel"))
                                 await Navigation.PushAsync(new LoadingDataPage());
                        }
                        else
                        {
                            if(await DisplayAlert("Warning", "Your password is out of date. You need to enter a new password to have updated information. Continue?", "Yes", "No"))
                            {
                                await Navigation.PushAsync(new MainPage());
                            }

                        }
                        break;

                }
            }
            else
            {
                await DisplayAlert("Warning", "The internet connection is needed to check updates", "OK");
            }
            AInd.IsEnabled = false;
            AInd.IsRunning = false;
            checkUpdater_button.IsEnabled = true;

        }

        private void langPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (langPicker.SelectedIndex == -1)
            {
                return;
            }
            else
            {
                string selectedLang = langPicker.Items[langPicker.SelectedIndex];
                LangResources.Culture = new CultureInfo(langsToPicker[selectedLang]);
            }
        }
    }
}
