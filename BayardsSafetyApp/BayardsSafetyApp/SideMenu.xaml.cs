﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BayardsSafetyApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SideMenu : ContentPage
    {

        public NavigationPage baseSections = new NavigationPage((new Sections
                        {
                           ParentSection = "null"
                        }))
                        {
                            BarBackgroundColor = (Color) Application.Current.Resources["myPrimaryColor"],
                            BarTextColor = Color.White
                        };
        public NavigationPage baseSettings = new NavigationPage((new SettingsPage()))
        {
            BarBackgroundColor = (Color)Application.Current.Resources["myPrimaryColor"],
            BarTextColor = Color.White
        };
        public NavigationPage baseMap;
        public SideMenu() //Side menu page
        {
            InitializeComponent();
            Title = AppReses.LangResources.Menu;
            baseMap = TabbedMapPage();

        }

        private void listView_ItemSelected(object sender, SelectedItemChangedEventArgs e) //Setting behavior on selection
        {
            if (e.SelectedItem == null)
            {
                return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
            }
            if (Parent != null && Parent is MasterDetailPage)
            {
                var a = ((MasterPageItem)e.SelectedItem).TargetType.Name;
                switch (a)
                {
                    case "Sections":
                        ((MasterDetailPage)Parent).Detail = baseSections;
                        break;
                    case "MapPage":
                        try
                        {
                            ((MasterDetailPage)Parent).Detail = baseMap;
                        }
                        catch (Exception ex)
                        {
                            DisplayAlert("Cannot show a map", ex.Message, "OK");
                        }

                        break;
                    case "SettingsPage":
                        
                        ((MasterDetailPage)Parent).Detail = new NavigationPage((new SettingsPage()))
                        {
                            BarBackgroundColor = (Color)Application.Current.Resources["myPrimaryColor"],
                            BarTextColor = Color.White
                        };
                        break;
                }
                listView.SelectedItem = null;
                if (Device.RuntimePlatform != Device.Windows)
                    ((MasterDetailPage)Parent).IsPresented = false;
            }
        }
        public void ChangeLang()
        {
            var masterPageItems = new List<MasterPageItem>();

            masterPageItems.Add(new MasterPageItem //Setting elements
            {
                Title = AppReses.LangResources.Contents,
                IconSource = "ic_home_bayards.png",
                TargetType = typeof(Sections)
            });         
            if (Device.RuntimePlatform == Device.iOS || Device.RuntimePlatform == Device.Android)
                masterPageItems.Add(new MasterPageItem
                {
                    Title = AppReses.LangResources.Locations,
                    IconSource = "ic_map_bayards.png",
                    TargetType = typeof(MapPage)
                });
            masterPageItems.Add(new MasterPageItem
            {
                Title = AppReses.LangResources.Settings,
                IconSource = "ic_settings_bayards.png",
                TargetType = typeof(SettingsPage)
            });

            listView.ItemsSource = masterPageItems;
        }
        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            var masterPageItems = new List<MasterPageItem>();

            masterPageItems.Add(new MasterPageItem //Setting elements
            {
                Title = AppReses.LangResources.Contents,
                IconSource = "ic_home_bayards.png",
                TargetType = typeof(Sections)
            });
            if (Device.RuntimePlatform == Device.iOS || Device.RuntimePlatform == Device.Android)
                masterPageItems.Add(new MasterPageItem
                {
                    Title = AppReses.LangResources.Locations,
                    IconSource = "ic_map_bayards.png",
                    TargetType = typeof(MapPage)
                });
            masterPageItems.Add(new MasterPageItem
            {
                Title = AppReses.LangResources.Settings,
                IconSource = "ic_settings_bayards.png",
                TargetType = typeof(SettingsPage)
            });

            listView.ItemsSource = masterPageItems;
        }

        private NavigationPage TabbedMapPage()
        {
            var locTabPage = new TabbedPage() { Title = "" };
            locTabPage.Children.Add(new MapPage());
            locTabPage.Children.Add(new LocationsPage());
            return new NavigationPage(locTabPage)
            {
                BarBackgroundColor = (Color)Application.Current.Resources["myPrimaryColor"],
                BarTextColor = Color.White
            };
        }
    }
    public class MasterPageItem //Class to describe the element in the side menu
    {
        public string Title { get; set; }
        public string IconSource { get; set; }
        public Type TargetType { get; set; }
    }
}
