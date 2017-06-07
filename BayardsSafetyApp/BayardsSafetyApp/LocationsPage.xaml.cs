using BayardsSafetyApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BayardsSafetyApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocationsPage : ContentPage
    {
        public LocationsPage()
        {
            InitializeComponent();
            Title = AppReses.LangResources.Locations;
        }
        private void SectionButton_Clicked(object sender, SelectedItemChangedEventArgs e) //Navigating to Section Content Page
        {
            if (e.SelectedItem == null)
            {
                return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
            }
            ((MapPage)((TabbedPage)Parent).Children[0]).FocusOn((Location)e.SelectedItem);
            ((TabbedPage)Parent).CurrentPage = ((TabbedPage)Parent).Children[0];
            //Navigation.PushAsync(new SectionContentPage(((Section)e.SelectedItem).Id_s, ((Section)e.SelectedItem).Name));
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {

            var _locations = Utils.DeserializeFromJson<List<Location>>((string)Application.Current.Properties["AllLocations"]).
                                    FindAll(l => l.Lang == AppReses.LangResources.Language).ToList();
            locView.ItemsSource = _locations;
            locView.SelectedItem = null;
        }
    }


}
