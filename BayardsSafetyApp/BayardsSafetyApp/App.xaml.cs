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
            var pageToStart = new NavigationPage();
            if (Current.Properties.ContainsKey("password"))
            {
                if (api.CheckInternetConnection())
                {
                    if (!Current.Properties.ContainsKey("UpdateTime") ||
                        !(Current.Properties.ContainsKey("AllSections") && Application.Current.Properties.ContainsKey("AllRisks")) ||
                            api.isUpdataNeeded((DateTime)Application.Current.Properties["UpdateTime"]).Result)
                    {
                        if(api.isPasswordCorrect((string)Current.Properties["password"]).Result)
                            pageToStart = new NavigationPage(new LoadingDataPage());
                        else
                            pageToStart = new NavigationPage(new MainPage());
                    }
                    else
                    {
                        pageToStart = new NavigationPage(new Sections
                        {
                            Contents = Utils.DeserializeFromJson<List<Section>>((string)Application.Current.Properties["AllSections"]).
                                        FindAll(s => s.Parent_s == "null" && s.Lang == AppReses.LangResources.Language).OrderBy(s => s.Name).ToList()
                        });
                    }

                }
                else if((Current.Properties.ContainsKey("AllSections") && Application.Current.Properties.ContainsKey("AllRisks")))
                    pageToStart = new NavigationPage(new Sections
                    {
                        Contents = ((List<Entities.Section>)Current.Properties["AllSections"]).
                                                        FindAll(s => s.Parent_s == "null" && s.Lang == AppReses.LangResources.Language)
                    });
                else
                    pageToStart = new NavigationPage(new MainPage());
            }
            else
                pageToStart = new NavigationPage(new MainPage());
            
            pageToStart.BarBackgroundColor = (Color)Application.Current.Resources["myPrimaryColor"];
            pageToStart.BarTextColor = Color.White;
            MainPage = pageToStart;
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
