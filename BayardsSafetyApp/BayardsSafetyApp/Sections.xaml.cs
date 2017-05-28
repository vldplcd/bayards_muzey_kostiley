using System;
using System.Collections.Generic;
using BayardsSafetyApp.Entities;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Linq;

namespace BayardsSafetyApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Sections : ContentPage
    {
        public Sections()
        {

            InitializeComponent();
            IsLoading = false;
            Title = AppReses.LangResources.Contents;
            
            var command = new Command(async () => { await Navigation.PushModalAsync(new SearchPage(this)); });
            ToolbarItems.Add(new ToolbarItem { Command = command, Text = AppReses.LangResources.Search });

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

        //protected override Boolean OnBackButtonPressed()
        //{
        //    base.OnBackButtonPressed();
        //    if(ParentSection == "null")
        //        return true;
        //    return false;
        //}
        private void SectionButton_Clicked(object sender, SelectedItemChangedEventArgs e)
        {
            IsLoading = true;
            if (e.SelectedItem == null)
            {
                return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
            }
            Navigation.PushAsync(new SectionContentPage(((Section)e.SelectedItem).Id_s, ((Section)e.SelectedItem).Name));
        }

        private void Sections_OnAppearing(object sender, EventArgs e)
        {
            if(Found != null)
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
