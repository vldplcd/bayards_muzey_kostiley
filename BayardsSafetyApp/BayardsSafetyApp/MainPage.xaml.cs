using BayardsSafetyApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BayardsSafetyApp
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            AInd.IsEnabled = false;
            AInd.IsRunning = false;

            BackgroundColor = Color.FromHex("#efefef");

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

        private string password = string.Empty;
        private async void ContinueButton_Clicked(object sender, EventArgs e)
        {
            AInd.IsEnabled = true;
            AInd.IsRunning = true;
            ContinueButton.IsEnabled = false;
            var AllSections = new Sections();
            API api = new API();
            if (api.CheckInternetConnection())
            {
                try
                {
                    await Task.Run(async () =>
                    {
                        var enc_password = MD5.GetMd5String(PasswordEntry.Text).Substring(0, 16);
                        try
                        {
                            if (await api.isPasswordCorrect(enc_password))
                            {
                                Application.Current.Properties["password"] = enc_password;
                                if (Application.Current.Properties.ContainsKey("LocAgr") && (bool)Application.Current.Properties["LocAgr"])
                                {
                                    AllSections.Contents = await LoadSections();
                                    throw new Exception("1");
                                }

                                else
                                    throw new Exception("2");
                            }
                            else
                            {
                                throw new Exception("Incorrect");
                            }
                        }
                        catch(Exception ex)
                        {
                            if (ex.Message == "1" || ex.Message == "2" || ex.Message == "Incorrect")
                                throw ex;
                            else
                                Device.BeginInvokeOnMainThread(() => { DisplayAlert(AppReses.LangResources.Warning, "The server is currently unavailable. " + ex.Message, "OK"); });
                        }
                    });

                }
                catch (TaskCanceledException)
                {
                    await DisplayAlert(AppReses.LangResources.Warning, AppReses.LangResources.ServerNoResp, AppReses.LangResources.OK);
                }
                catch (ArgumentException)
                {

                }
                catch (Exception ex)
                {
                    if (ex.Message.StartsWith("Incorrect"))
                        await DisplayAlert(AppReses.LangResources.Warning, AppReses.LangResources.IncPassword, AppReses.LangResources.OK);
                    if (ex.Message.StartsWith("1"))
                    {
                        var mp = GetMasterPage();
                        mp.Detail = new NavigationPage(AllSections) {
                            BarBackgroundColor = (Color)Application.Current.Resources["myPrimaryColor"],
                            BarTextColor = Color.White
                        };
                        App.Current.MainPage = mp;
                    }

                    if (ex.Message.StartsWith("2"))
                        await Navigation.PushAsync(new LocalePage());
                    if (ex.Message.StartsWith("3"))
                        if (await DisplayAlert(AppReses.LangResources.Warning,
                        AppReses.LangResources.DownloadWarn,
                        AppReses.LangResources.OK, AppReses.LangResources.Cancel))
                        {
                            await Navigation.PushAsync(new LoadingDataPage());
                        }

                }
            }
            else
            {
                await DisplayAlert(AppReses.LangResources.Warning,
                    AppReses.LangResources.NoIntConn, AppReses.LangResources.OK);
            }
            AInd.IsEnabled = false;
            AInd.IsRunning = false;
            ContinueButton.IsEnabled = true;

        }

        private void PasswordEntry_Completed(object sender, EventArgs e)
        {
            password = ((Entry)sender).Text;
            ContinueButton_Clicked(sender, e);
        }

        private async Task<List<Section>> LoadSections()
        {
            API api = new API();
            List<Section> contents = new List<Section>();
            if (!Application.Current.Properties.ContainsKey("UpdateTime") ||
                !(Application.Current.Properties.ContainsKey("AllSections") && Application.Current.Properties.ContainsKey("AllRisks")) ||
                api.isUpdataNeeded((DateTime)Application.Current.Properties["UpdateTime"]).Result)
            {
                throw new Exception("3");
            }
            else
            {
                //contents = App.Database.SectionDatabase.GetItems<Section>().ToList().FindAll(s => s.Parent_s == "null"
                //                                                                        && s.Lang == AppResources.LangResources.Language).
                //                                                                        OrderBy(s => s.Name).ToList();
                contents = Utils.DeserializeFromJson<List<Section>>((string)Application.Current.Properties["AllSections"]).
                    FindAll(s => s.Parent_s == "null" && s.Lang == AppReses.LangResources.Language).OrderBy(s => s.Order).ThenBy(s => s.Name).ToList();
            }

            return contents;
        }
        private MasterDetailPage GetMasterPage()
        {
            var mp = new MasterDetailPage();
            mp.Master = new SideMenu();
            mp.IsPresented = false;
            mp.MasterBehavior = MasterBehavior.Popover;
            return mp;
        }
    }
}
