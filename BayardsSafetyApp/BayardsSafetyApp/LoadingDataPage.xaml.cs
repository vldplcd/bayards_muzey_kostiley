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
        protected override Boolean OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            return true;
        }
        private void SetProgress(double pr)
        {
            ProgressState = pr;
            if(pr == 1)
            {
                Application.Current.Properties["UpdateTime"] = DateTime.Now;
                Application.Current.SavePropertiesAsync().Wait();
                Cont.Contents = Utils.DeserializeFromJson<List<Section>>((string)Application.Current.Properties["AllSections"]).
                   FindAll(s => s.Parent_s == "null"&&s.Lang == AppReses.LangResources.Language).OrderBy(s => s.Name).ToList();
                //Cont.Contents = App.Database.SectionDatabase.GetItems<Section>().ToList();
                
            }
                
        }
        double _progressState;
        public double ProgressState
        {
            get { return _progressState; }
            set
            {
                _progressState = value;
                OnPropertyChanged();
            }
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            Device.StartTimer(new TimeSpan(0, 0, 1), OnTimerToComplete);
            LoadData();
            
        }
        public void LoadData()
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
                    catch (Exception ex)
                    {
                        Device.BeginInvokeOnMainThread(() => 
                        {
                            DisplayAlert("Error", "Loading exception: "+ex.Message, "OK");
                            TryAgain_Button.IsEnabled = true;
                            TryAgain_Button.IsVisible = true;
                            AInd.IsVisible = false;
                            
                        });                        
                    }
                    
                });

            }
            catch (TaskCanceledException)
            {
                DisplayAlert("Error", "A server does not respond", "OK");
                TryAgain_Button.IsEnabled = true;
                TryAgain_Button.IsVisible = true;

            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    default:
                        DisplayAlert("Error", ex.Message, "OK");
                        TryAgain_Button.IsEnabled = true;
                        TryAgain_Button.IsVisible = true;
                        break;
                }
            }
        }
        private bool OnTimerToComplete()
        {
            PrBar.Progress = ld.Process;
            if(ld.Process == 1)
            {
                Application.Current.Properties["UpdateTime"] = DateTime.Now;
                //Application.Current.SavePropertiesAsync().Wait();
                //using (var context = App.Database)
                //{
                //    Cont.Contents = App.Database.SectionDatabase.GetItems<Section>().ToList().FindAll(s => s.Parent_s == "null"
                //                                                                        && s.Lang == AppResources.LangResources.Language).
                //                                                                        OrderBy(s => s.Name).ToList();
                //}                    
                Cont.Contents = Utils.DeserializeFromJson<List<Section>>((string)Application.Current.Properties["AllSections"]).
                    FindAll(s => s.Parent_s == "null" && s.Lang == AppReses.LangResources.Language).OrderBy(s => s.Name).ToList();
                Navigation.PushAsync(Cont);
                return false;
            }
            return true;
        }

        private void TryAgain_Button_Clicked(object sender, EventArgs e)
        {
            TryAgain_Button.IsEnabled = false;
            TryAgain_Button.IsVisible = true;
            LoadData();
        }
    }
}
