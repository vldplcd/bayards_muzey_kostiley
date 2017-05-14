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
                Cont.Contents = ((List<Section>)Application.Current.Properties["AllSections"]).
                    FindAll(s => s.Parent_s == "null"&&s.Lang == AppResources.LangResources.Language).OrderBy(s => s.Name).ToList();
                
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
            try
            {
                var load = Task.Run(() => {
                    try
                    {
                        ld.ToDatabase().Wait();
                    }
                    catch(Exception ex)
                    {
                        DisplayAlert("Error", "A server does not respond", "OK");
                    }
                        
                });
            }
            catch (TaskCanceledException)
            {
                DisplayAlert("Error", "A server does not respond", "OK");
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    default:
                        DisplayAlert("Error", ex.Message, "OK");
                        break;
                }
            }
            
        }

        private bool OnTimerToComplete()
        {
            if(ld.Process == 1)
            {
                Application.Current.Properties["UpdateTime"] = DateTime.Now;
                Application.Current.SavePropertiesAsync().Wait();
                Cont.Contents = ((List<Section>)Application.Current.Properties["AllSections"]).
                    FindAll(s => s.Parent_s == "null" && s.Lang == AppResources.LangResources.Language).OrderBy(s => s.Name).ToList();
                Navigation.PushAsync(Cont);
                return false;
            }
            return true;
        }
            private bool OnTimerToLoad()
        {
            try
            {
                var load = Task.Run(async () => {
                    await ld.ToDatabase();
                    
                });
                while (!load.IsCompleted) { }
                Navigation.PushAsync(Cont);
                //var Cont = new Sections();
                //string databasePath = DependencyService.Get<ISQLite>().GetDatabasePath("bayards.db");
                //List<Risk> a;
                //List<Media> b;
                //using (var context = new SQLiteConnection(databasePath))
                //{
                //    Cont.Contents = context.Table<Section>().ToList();
                //    a = context.Table<Risk>().ToList();
                //}

                //Cont.Contents = (List<Section>)Application.Current.Properties["AllSections"];
                //await Navigation.PushAsync(Cont);
                //while (load.Status == TaskStatus.Running || load.Status == TaskStatus.WaitingToRun || load.Status == TaskStatus.WaitingForActivation)
                //{
                //    PrBar.Progress = ld.Process;
                //}
            }
            catch(Exception ex)
            {
                switch (ex.Message)
                {
                    default:
                        DisplayAlert("Error", ex.Message, "OK");
                        break;
                }
            }
            return false;
        }
    }
}
