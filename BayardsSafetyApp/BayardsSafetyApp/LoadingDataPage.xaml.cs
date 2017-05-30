using BayardsSafetyApp.Entities;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BayardsSafetyApp
{
    //This page is viewed when the data is loading

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingDataPage : ContentPage
    {
        public LoadingDataPage()
        {
            InitializeComponent();
            TryAgain_Button.IsVisible = false;
            
        }

        DBLoading.LoadData ld = new DBLoading.LoadData();
        Sections Cont = new Sections();
        protected override Boolean OnBackButtonPressed() //disable back button
        {
            base.OnBackButtonPressed();
            return true;
        }


        private void ContentPage_Appearing(object sender, EventArgs e) //method on page appearing
        {
            Device.StartTimer(new TimeSpan(0, 0, 1), OnTimerToComplete); //starts timer to check progress
            LoadData(); //starts loading data method
            
        }
        public void LoadData() //loading data method
        {
            TryAgain_Button.IsEnabled = false;
            try
            {
                var load = Task.Run(() => {
                    try
                    {
                        ld.ToDatabase().Wait();
                        ld.Process = 1;
                    }
                    catch (Exception ex) //on server errors exception behaviour
                    {
                        Device.BeginInvokeOnMainThread(() => 
                        {
                            DisplayAlert(AppReses.LangResources.Error, "Loading exception: "+ex.Message, AppReses.LangResources.OK);
                            TryAgain_Button.IsEnabled = true;
                            TryAgain_Button.IsVisible = true;
                            AInd.IsVisible = false;
                            if (ex.Message.Contains("api"))
                            {
                                DisplayAlert(AppReses.LangResources.Error, "Reenter password", AppReses.LangResources.OK);
                                App.Current.MainPage = new MainPage();
                            }
                                

                        });
                        
                    }
                    
                });

            }
            catch (TaskCanceledException)//or server errors exception behaviour
            {
                DisplayAlert(AppReses.LangResources.Error, AppReses.LangResources.ServerNoResp,
                    AppReses.LangResources.OK);
                TryAgain_Button.IsEnabled = true;
                TryAgain_Button.IsVisible = true;

            }
            catch (Exception ex) //on any other exception behaviour
            {
                switch (ex.Message)
                {
                    default:
                        DisplayAlert(AppReses.LangResources.Error, 
                            ex.Message, AppReses.LangResources.OK);
                        TryAgain_Button.IsEnabled = true;
                        TryAgain_Button.IsVisible = true;
                        break;
                }
            }
        }
        private bool OnTimerToComplete() //method to check progress every 2 sec
        {
            PrBar.Progress = ld.Process;
            if(ld.Process == 1) //if loading is done
            {
                Application.Current.Properties["UpdateTime"] = DateTime.Now;
                Cont.ParentSection = "null";
                var mp = GetMasterPage();
                mp.Detail = new NavigationPage(Cont) {
                    BarBackgroundColor = (Color)Application.Current.Resources["myPrimaryColor"],
                    BarTextColor = Color.White
                };
                try
                {
                    App.Current.MainPage = mp;
                }
                catch (Exception) { }
                return false;
            }
            return true;
        }
        private MasterDetailPage GetMasterPage() //creating page with side menu
        {
            var mp = new MasterDetailPage();
            mp.Master =new SideMenu();
            return mp;
        }

        private void TryAgain_Button_Clicked(object sender, EventArgs e)
        {
            TryAgain_Button.IsEnabled = false;
            TryAgain_Button.IsVisible = true;
            LoadData();
        }

    }
}
