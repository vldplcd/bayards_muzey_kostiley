using System;
using System.Collections.Generic;
using BayardsSafetyApp.Entities;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Linq;

namespace BayardsSafetyApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Sections : ContentPage //right tab in section contents page and also the first page after loading
    {
        ToolbarItem searchToolbar;
        public Sections() 
        {

            InitializeComponent();
            IsLoading = false;
            Title = AppReses.LangResources.Contents;
            
            var command = new Command(async () => { await Navigation.PushModalAsync(new SearchPage(this)); });
            searchToolbar = new ToolbarItem { Command = command, Text = AppReses.LangResources.Search };
            ToolbarItems.Add(searchToolbar);

        }
        public Page Found { get; set; }
        public string ParentSection { get; set; }
        List<Section> _contents;
        public List<Section> Contents
        {
            get { return _contents; }
            set
            {
                _contents = value;
                OnPropertyChanged();
            }
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

        private void SectionButton_Clicked(object sender, SelectedItemChangedEventArgs e) //Navigating to Section Content Page
        {
            IsLoading = true;
            if (e.SelectedItem == null)
            {
                return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
            }
            Navigation.PushAsync(new SectionContentPage(((Section)e.SelectedItem).Id_s, ((Section)e.SelectedItem).Name));
        }

        private void Sections_OnAppearing(object sender, EventArgs e) //on appearing having section list.
        {
            searchToolbar.Text = AppReses.LangResources.Search;
            Title = AppReses.LangResources.Contents;
            if (Found != null)
            {
                Navigation.PushAsync(Found);
                Found = null;
            }
            ParentSection = ParentSection == null ? "null" : ParentSection;
            
            _contents = Utils.DeserializeFromJson<List<Section>>((string)Application.Current.Properties["AllSections"]).
                                    FindAll(s => s.Parent_s == ParentSection && s.Lang == AppReses.LangResources.Language).OrderBy(s => s.Order).ThenBy(s => s.Name).ToList();
            sectView.ItemsSource = _contents;
            sectView.SelectedItem = null;
        }


    }
}
