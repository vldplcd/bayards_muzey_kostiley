using BayardsSafetyApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace BayardsSafetyApp
{
    public partial class App : Application
    {
        private static DataBaseUOW _database;
        public static DataBaseUOW Database => _database ?? (_database = new DataBaseUOW());

        public App()
        {
            InitializeComponent();
            API api = new API();
            if (Device.RuntimePlatform == Device.Android || Device.RuntimePlatform == Device.iOS)
            {
                //var ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
                //LangResources.Culture = ci; // set the RESX for resource localization
                //DependencyService.Get<ILocalize>().SetLocale(ci); // set the Thread for locale-aware methods
            }
            var isWithoutLoad = false;
            var pageToStart = new Page();
            if (Current.Properties.ContainsKey("password"))
            {
                if (api.CheckInternetConnection())
                {
                    if (!Current.Properties.ContainsKey("UpdateTime") ||
                        !(Current.Properties.ContainsKey("AllSections") && Application.Current.Properties.ContainsKey("AllRisks")) ||
                            api.isUpdataNeeded((DateTime)Application.Current.Properties["UpdateTime"]).Result)
                    {
                        if (api.isPasswordCorrect((string)Current.Properties["password"]).Result)
                            pageToStart = new NavigationPage(new LoadingDataPage());
                        else
                            pageToStart = new NavigationPage(new MainPage());
                    }
                    else
                    {
                        var mp = GetMasterPage();
                        
                        mp.Detail = new NavigationPage(new Sections
                        {
                            ParentSection = "null"
                        })
                        {
                            BarBackgroundColor = (Color)Application.Current.Resources["myPrimaryColor"],
                            BarTextColor = Color.White
                        };
                        isWithoutLoad = true;
                        pageToStart = mp;
                    }

                }
                else if ((Current.Properties.ContainsKey("AllSections") && Application.Current.Properties.ContainsKey("AllRisks")))
                {
                    var mp = GetMasterPage();
                    mp.Detail = new NavigationPage(new Sections
                    {
                        ParentSection = "null"
                    })
                    {
                        BarBackgroundColor = (Color)Application.Current.Resources["myPrimaryColor"],
                        BarTextColor = Color.White
                    };
                    
                    isWithoutLoad = true;
                    pageToStart = mp;
                }
                else
                    pageToStart = new NavigationPage(new MainPage());
            }
            else
                pageToStart = new NavigationPage(new MainPage());
            if (!isWithoutLoad)
            {
                ((NavigationPage)pageToStart).BarBackgroundColor = (Color)Application.Current.Resources["myPrimaryColor"];
                ((NavigationPage)pageToStart).BarTextColor = Color.White;
            }
            
            MainPage = pageToStart;
        }

        public void ChangeMainPage(Page page)
        {
            MainPage = page;
        }

        private MasterDetailPage GetMasterPage()
        {
            var mp = new MasterDetailPage();
            mp.Master = new SideMenu();
            mp.IsPresented = false;
            mp.MasterBehavior = MasterBehavior.Popover;
            return mp;
        }
        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            //Application.Current.SavePropertiesAsync().Wait();
            // Handle when your app sleeps
        }
        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
