using BayardsSafetyApp.AppReses;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BayardsSafetyApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage //Settings page
    {
        Dictionary<string, string> langsToPicker;
        public List<string> PickerContent { get; set; }
        public SettingsPage()
        {
            InitializeComponent();
            Title = LangResources.Settings;
            langsToPicker = new Dictionary<string, string>
                    {
                        {AppReses.LangResources.NlButton,  "nl"}, {AppReses.LangResources.EnButton, "en"} //Setting language selector values
                    };
            langPicker.ItemsSource = langsToPicker.Keys.ToList();
            checkUpdater_button.Text = LangResources.CheckUpd;
            langPicker.Title = AppReses.LangResources.ChooseLang; //Setting language selector text
            updMessage.Text = $"{AppReses.LangResources.LastUpd}: {((DateTime)Application.Current.Properties["UpdateTime"]).ToString()}.";//Setting last update time
        }

        private async void checkUpdater_button_Clicked(object sender, EventArgs e) //behavior on check updates click
        {
            AInd.IsEnabled = true;
            AInd.IsRunning = true;
            checkUpdater_button.IsEnabled = false;
            API api = new API();
            if (api.CheckInternetConnection())
            {
                await Task.Run(() =>
                {
                    try
                    {
                        switch ((string)checkUpdater_button.CommandParameter)
                        {
                            case "check":
                                if (api.isUpdataNeeded(((DateTime)Application.Current.Properties["UpdateTime"])).Result)
                                {
                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        checkUpdater_button.Text = AppReses.LangResources.UpdButton;
                                        checkUpdater_button.CommandParameter = "update";
                                        updMessage.Text = LangResources.UpdAvailable;
                                    });
                                }
                                else
                                {
                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        updMessage.Text = $"Last update: {((DateTime)Application.Current.Properties["UpdateTime"]).ToString()}. No new updates found";
                                    });
                                }
                                break;
                            case "update":
                                if (api.isPasswordCorrect((string)Application.Current.Properties["password"]).Result)
                                {
                                    Device.BeginInvokeOnMainThread(async () =>
                                    {
                                        if (await DisplayAlert(AppReses.LangResources.Warning, LangResources.DownloadWarn,
                                        AppReses.LangResources.OK, AppReses.LangResources.Cancel))
                                            await Navigation.PushAsync(new LoadingDataPage());
                                    });
                                }
                                else
                                {
                                    Device.BeginInvokeOnMainThread(async () =>
                                    {
                                        if (await DisplayAlert(AppReses.LangResources.Warning, LangResources.OutdatedPassw,
                                            AppReses.LangResources.Yes, AppReses.LangResources.No))
                                        {
                                            await Navigation.PushAsync(new MainPage());
                                        }
                                    });

                                }
                                break;

                        }
                    }
                    catch (Exception ex)
                    {
                        Device.BeginInvokeOnMainThread(() => { DisplayAlert(AppReses.LangResources.Warning, "The server is currently unavailable. " + ex.Message, "OK"); });
                    }
                });
                


            }
            else
            {
                await DisplayAlert(AppReses.LangResources.Warning, AppReses.LangResources.NoIntConnUpd, AppReses.LangResources.OK);
            }
            AInd.IsEnabled = false;
            AInd.IsRunning = false;
            checkUpdater_button.IsEnabled = true;

        }

        private void langPicker_SelectedIndexChanged(object sender, EventArgs e) //behavior on changing language
        {
            if (langPicker.SelectedIndex == -1)
            {
                return;
            }
            else
            {
                string selectedLang = langPicker.Items[langPicker.SelectedIndex];
                LangResources.Culture = new CultureInfo(langsToPicker[selectedLang]);
                App.Current.Properties["lang"] = langsToPicker[selectedLang];
                if (Parent.Parent != null && Parent.Parent.GetType() == typeof(MasterDetailPage))
                {
                    ((SideMenu)((MasterDetailPage)Parent.Parent).Master).ChangeLang();
                }
                Title = LangResources.Settings;
                checkUpdater_button.Text = LangResources.CheckUpd;
                checkUpdater_button.CommandParameter = "check";
                updMessage.Text = $"{AppReses.LangResources.LastUpd}: {((DateTime)Application.Current.Properties["UpdateTime"]).ToString()}.";//Setting last update time
            }
        }

        private void ResetPicker()
        {

        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {

        }
    }
}
