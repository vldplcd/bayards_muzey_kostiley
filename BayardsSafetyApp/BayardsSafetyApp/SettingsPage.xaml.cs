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
            {AppReses.LangResources.NlButton,  "nl"}, {AppReses.LangResources.EnButton, "en"}
        };
        public SettingsPage()
        {
            InitializeComponent();
            langPicker.Title = AppReses.LangResources.ChooseLang;
            langPicker.ItemsSource = langsToPicker.Keys.ToList();
            updMessage.Text = $"{AppReses.LangResources.LastUpd}: {((DateTime)Application.Current.Properties["UpdateTime"]).ToString()}.";
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
                            checkUpdater_button.Text = AppReses.LangResources.UpdButton;
                            checkUpdater_button.CommandParameter = "update";
                            updMessage.Text = LangResources.UpdAvailable;
                        }
                        else
                        {
                            updMessage.Text = $"Last update: {((DateTime)Application.Current.Properties["UpdateTime"]).ToString()}. No new updates found";
                        }
                        break;
                    case "update":
                        if(api.isPasswordCorrect((string)Application.Current.Properties["password"]).Result)
                        {
                            if (await DisplayAlert(AppReses.LangResources.Warning, LangResources.DownloadWarn,
                                AppReses.LangResources.OK, AppReses.LangResources.Cancel))
                                 await Navigation.PushAsync(new LoadingDataPage());
                        }
                        else
                        {
                            if(await DisplayAlert(AppReses.LangResources.Warning, LangResources.OutdatedPassw,
                                AppReses.LangResources.Yes, AppReses.LangResources.No))
                            {
                                await Navigation.PushAsync(new MainPage());
                            }

                        }
                        break;

                }
            }
            else
            {
                await DisplayAlert(AppReses.LangResources.Warning, AppReses.LangResources.NoIntConnUpd, AppReses.LangResources.OK);
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
