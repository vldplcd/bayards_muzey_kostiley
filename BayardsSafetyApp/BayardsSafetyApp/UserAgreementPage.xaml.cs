using BayardsSafetyApp.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
//using Android.Media;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BayardsSafetyApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserAgreementPage : ContentPage //User Agreement Page
    {
        public UserAgreementPage()
        {
            InitializeComponent();
            AgrmntLabel.Text = AppReses.LangResources.AgrmntLabel;
            ContinueButton.Text = AppReses.LangResources.ContinueButton;
            UserAgrLabel.Text = AppReses.LangResources.UserAgreementText;
        }
        protected override Boolean OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            return true;
        }
        private async void ContinueButton_Clicked(object sender, EventArgs e) //Clicking "I agree" button. Navigating to loading.
        {
            Application.Current.Properties["LocAgr"] = true;
            AInd.IsEnabled = true;
            AInd.IsRunning = true;
            var b = Application.Current.Properties["LocAgr"];
            ContinueButton.IsEnabled = false;
            var AllSections = new Sections();
            try
            {
                await Task.Run(() =>
                {

                    var api = new API();
                    AllSections.Contents = LoadSections();
                    throw new Exception("1");
                });

            }
            catch (TaskCanceledException)
            {
                await DisplayAlert("Warning", "The server doesn't respond", "OK");
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("1"))
                    await Navigation.PushAsync(AllSections);
                if (ex.Message.StartsWith("3"))
                    if (await DisplayAlert(AppReses.LangResources.Warning,
                    AppReses.LangResources.DownloadWarn,
                    AppReses.LangResources.OK, AppReses.LangResources.Cancel))
                        await Navigation.PushAsync(new LoadingDataPage());
            }
            AInd.IsEnabled = false;
            AInd.IsRunning = false;
            ContinueButton.IsEnabled = true;
        }

        private List<Section> LoadSections()
        {
            List<Section> contents = new List<Section>();
            if (!Application.Current.Properties.ContainsKey("UpdateTime") || (Application.Current.Properties.ContainsKey("UpdateTime") &&
                (DateTime)Application.Current.Properties["UpdateTime"] < DateTime.MaxValue))
            {

                throw new Exception("3");
            }
            else
            {
                throw new Exception("3");
            }
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            API api = new API();
            try
            {
                if (api.CheckInternetConnection())
                    UserAgrLabel.Text = api.GetUserAgreement(AppReses.LangResources.Language);
                else
                {
                    DisplayAlert(AppReses.LangResources.Warning, "Cannot load user agreement. The internet connection is required.  Returning to the first screen", "OK");
                    //App.Current.MainPage = new MainPage();
                }
            }
            catch(Exception ex)
            {
                DisplayAlert(AppReses.LangResources.Warning, "Error occured: " + ex.Message + " Returning to the first screen", "OK");
                App.Current.MainPage = new MainPage();
            }

        }
    }
}