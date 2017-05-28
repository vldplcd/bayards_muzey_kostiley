using BayardsSafetyApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BayardsSafetyApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SideMenu : ContentPage
    {
        public SideMenu()
        {
            InitializeComponent();
            Title = "Menu";
            var masterPageItems = new List<MasterPageItem>();
            masterPageItems.Add(new MasterPageItem
            {
                Title = "Contents",
                //IconSource = "contents.png",
                TargetType = typeof(Sections)
            });
            //masterPageItems.Add(new MasterPageItem
            //{
            //    Title = "Map of locations",
            //    //IconSource = "map.png",
            //    //TargetType = typeof(MapPage)
            //});
            masterPageItems.Add(new MasterPageItem
            {
                Title = "Settings",
                //IconSource = "settings.png",
                TargetType = typeof(SettingsPage)
            });

            listView.ItemsSource = masterPageItems;
        }

        protected override Boolean OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            return true;
        }

        private void listView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
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
                        ((MasterDetailPage)Parent).Detail = new NavigationPage((new Sections
                        {
                            Contents = Utils.DeserializeFromJson<List<Section>>((string)Application.Current.Properties["AllSections"]).
                        FindAll(s => s.Parent_s == "null" && s.Lang == AppReses.LangResources.Language).OrderBy(s => s.Name).ToList()
                        }));
                        break;
                    //case "MapPage":
                    //    try
                    //    {
                    //        ((MasterDetailPage)Parent).Detail = new MapPage();
                    //    }
                    //    catch(Exception ex)
                    //    {
                    //        DisplayAlert("Cannot show a map", ex.Message, "OK");
                    //    }

                    //    break;
                    case "SettingsPage":
                        ((MasterDetailPage)Parent).Detail = new NavigationPage((new SettingsPage()));
                        break;
                }
                listView.SelectedItem = null;
                if (Device.RuntimePlatform != Device.Windows)
                    ((MasterDetailPage)Parent).IsPresented = false;
            }
        }
    }
    public class MasterPageItem
    {
        public string Title { get; set; }
        public string IconSource { get; set; }
        public Type TargetType { get; set; }
    }
}
