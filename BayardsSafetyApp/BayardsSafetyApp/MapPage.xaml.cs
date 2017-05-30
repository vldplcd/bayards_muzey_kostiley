using BayardsSafetyApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace BayardsSafetyApp
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        Geocoder geoCoder;
        List<Location> _locations;
        public MapPage()
        {

            InitializeComponent();
            geoCoder = new Geocoder();
            _locations = Utils.DeserializeFromJson<List<Location>>((string)Application.Current.Properties["AllLocations"]).
                                    FindAll(l => l.Lang == AppReses.LangResources.Language).ToList();
            foreach (var loc in _locations)
            {
                MyMap.Pins.Add(new Pin
                {
                    Label = loc.Name,
                    Position = new Position(loc.Latitude, loc.Longitude),
                    Type = PinType.Generic
                });
            }
            var zoomLevel = SliderZoom.Value; // between 1 and 18
            var latlongdegrees = 360 / (Math.Pow(2, zoomLevel));
            if (_locations.Count != 0)
                MyMap.MoveToRegion(new MapSpan(new Position(_locations[0].Latitude, _locations[0].Longitude), latlongdegrees, latlongdegrees));
        }

        private async void OnAddPinClicked(object sender, EventArgs e)
        {
            var point = MyMap.VisibleRegion.Center;
            var item = (await geoCoder.GetAddressesForPositionAsync(point)).FirstOrDefault();

            var name = item ?? "Unknown";

            MyMap.Pins.Add(new Pin
            {
                Label = name,
                Position = point,
                Type = PinType.Generic
            });
        }
        private void OnStreetClicked(object sender, EventArgs e) =>
            MyMap.MapType = MapType.Street;

        private void OnHybridClicked(object sender, EventArgs e) =>
            MyMap.MapType = MapType.Hybrid;

        private void OnSatelliteClicked(object sender, EventArgs e) =>
            MyMap.MapType = MapType.Satellite;

        private void OnSliderChanged(object sender, ValueChangedEventArgs e)
        {
            var zoomLevel = e.NewValue; // between 1 and 18
            var latlongdegrees = 360 / (Math.Pow(2, zoomLevel));
            MyMap.MoveToRegion(new MapSpan(MyMap.VisibleRegion.Center, latlongdegrees, latlongdegrees));
        }
    }
}
