using BayardsSafetyApp.Entities;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Linq;

namespace BayardsSafetyApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RiskDetails
    {
        public string RiskId { get; set; }

        private List<Media> imagesList;
        private List<Media> videosList;
        double _width;
        public RiskDetails(Risk risk)
        {
            InitializeComponent();
            pictView.IsVisible = false;
            videoView.IsVisible = false;
            AgreeLabel.Text = AppReses.LangResources.ContinueButton;
            BackgroundColor = Color.FromHex("#efefef");
            Header.Text = risk.Name;
            RiskId = risk.Id_r;
            textDetails.Text = risk.Content;
            showImagesButton.Text = AppReses.LangResources.ShowImages;
            showVideosButton.Text = AppReses.LangResources.ShowVideo;
            var allMedia = Utils.DeserializeFromJson<List<Media>>((string)Application.Current.Properties["AllMedia"]).FindAll(m => m.Id_r == risk.Id_r
                                                                                       && m.Lang == AppReses.LangResources.Language);
            var temp_imagesList = allMedia.FindAll(m => m.Type == "image");
            imagesList = new List<Media>();
            foreach (var im in temp_imagesList)
                imagesList.Add(new Media { Id_r = im.Id_r, Lang = im.Lang, Type = im.Type, Url = im.Url });
            videosList = allMedia.FindAll(m => m.Type == "video");
            pictView.ItemsSource = imagesList;
            videoView.ItemsSource = videosList;
        }
        bool _isImagesListShown = false;
        private bool IsImagesListShown
        {
            get
            {
                return _isImagesListShown;
            }
            set{
                if (value)
                {
                    if (imagesList != null && imagesList.Count != 0)
                    {
                        riskGrid.RowDefinitions[3].Height = new GridLength(10, GridUnitType.Auto);
                        _width = scrView.Width * 0.7;
                        foreach (var im in imagesList)
                            im.Width = _width;
                        pictView.ItemsSource = imagesList;
                        pictView.IsVisible = true;
                        showImagesButton.Text = AppReses.LangResources.HideImages;
                    }
                }
                else
                {
                    riskGrid.RowDefinitions[3].Height = new GridLength(0, GridUnitType.Absolute);
                    pictView.IsVisible = false;
                    showImagesButton.Text = AppReses.LangResources.ShowImages;
                }
                _isImagesListShown = value;
            }
        }
        private void ShowImagesButton_Clicked(object sender, EventArgs e)
        {
            IsImagesListShown = !IsImagesListShown;
        }

        bool _isVideosListShown = false;
        private bool IsVideosListShown
        {
            get
            {
                return _isVideosListShown;
            }
            set
            {
                if (value)
                {
                    if(videoView.ItemsSource != null && videosList.Count != 0)
                    {
                        riskGrid.RowDefinitions[5].Height = new GridLength(10, GridUnitType.Auto);
                        videoView.IsVisible = true;
                        showVideosButton.Text = AppReses.LangResources.HideVideo;
                    }
                }
                else
                {
                    riskGrid.RowDefinitions[5].Height = new GridLength(0, GridUnitType.Absolute);
                    videoView.IsVisible = false;
                    showVideosButton.Text = AppReses.LangResources.ShowVideo;
                }
                _isVideosListShown = value;
            }
        }
        private void ShowVideosButton_Clicked(object sender, EventArgs e)
        {
            IsVideosListShown = !IsVideosListShown;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://www.youtube.com/embed/" + (string)((Button)sender).CommandParameter));
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
        }

        private void pictView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            pictView.SelectedItem = null;
        }
    }
}
