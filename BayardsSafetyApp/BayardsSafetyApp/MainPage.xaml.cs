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
            try
            {                
                await Task.Run(async () =>
                {

                    var api = new API();
                    if (api.isPasswordCorrect(PasswordEntry.Text))
                    {
                        bool b;
                        try
                        {
                            if (Application.Current.Properties.ContainsKey("LocAgr"))
                                b = (bool)Application.Current.Properties["LocAgr"];
                        }
                        catch(Exception ex)
                        {

                        }
                        
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
                });
                
            }
            catch(TaskCanceledException)
            {
                await DisplayAlert("Warning", "The server doesn't respond", "OK");
            }
            catch(Exception ex)
            {
                if(ex.Message.StartsWith("Incorrect"))
                    await DisplayAlert("Warning", "The password is incorrect", "OK");
                if (ex.Message.StartsWith("1"))
                    await Navigation.PushAsync(AllSections);
                if (ex.Message.StartsWith("2"))
                    await Navigation.PushAsync(new LocalePage());
                if (ex.Message.StartsWith("3"))
                    if (await DisplayAlert("Warning",
                    "The information has been updated. Now the app will use the internet connection to download new data. Please be aware that there may be a charge for data transfer over the mobile network.",
                    "OK", "Cancel"))
                        await Navigation.PushAsync(new LoadingDataPage());
                //await DisplayAlert("Warning", ex.Message, "OK");
            }
            AInd.IsEnabled = false;
            AInd.IsRunning = false;
            ContinueButton.IsEnabled = true;

        }

        private void PasswordEntry_Completed(object sender, EventArgs e)
        {
            password = ((Entry)sender).Text;
        }

        private async Task<List<Section>> LoadSections()
        {
            List<Section> contents = new List<Section>();
            if (!Application.Current.Properties.ContainsKey("UpdateTime") || (Application.Current.Properties.ContainsKey("UpdateTime") &&
                (DateTime)Application.Current.Properties["UpdateTime"] < DateTime.MaxValue))
            {
                    throw new Exception("3");
            }
            else
            {
                contents = ((List<Section>)Application.Current.Properties["AllSections"]).
                    FindAll(s => s.Parent_s == "null" && s.Lang == AppResources.LangResources.Language).OrderBy(s => s.Name).ToList();
            }
            
                //await App.Database.CreateTable<Media>();
                //await App.Database.CreateTable<Risk>();
                //await App.Database.CreateTable<SafetyObject>();
                //await App.Database.CreateTable<Section>();
                //await App.Database.CreateTable<SectionContents>();
                //foreach (var item in contents)
                //{
                //    await App.Database.InsertItemAsync(item);
                //}
            return contents;
        }

        private void SearchBar_Activated(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new SearchPage());
        }
    }
}
