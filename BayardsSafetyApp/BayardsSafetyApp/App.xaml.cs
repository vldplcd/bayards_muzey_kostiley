﻿using System;
using System.Globalization;
using Xamarin.Forms;

namespace BayardsSafetyApp
{
    //This code is done when the app starts
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            //.............................ENTER THE SERVER ADDRESS HERE..................................
            Current.Properties["host"] = "http://vhost29450.cpsite.ru";
            //............................................................................................
            if(App.Current.Properties.ContainsKey("lang"))
                AppReses.LangResources.Culture = new CultureInfo((string)App.Current.Properties["lang"]);
            API api = new API();
            var isWithoutLoad = false;
            var pageToStart = new Page();
            if (Current.Properties.ContainsKey("password")) //checking if the password has been entered
            {
                if (api.CheckInternetConnection()) //checking connection then loading page according to updates and password state
                {
                    try
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
                    catch(Exception)
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
                    mp.Detail = ((SideMenu)mp.Master).baseSections;
                    
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
