using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Acr.UserDialogs;
using GooMaps.Models;

namespace GooMaps
{
    public partial class MainPage : ContentPage
    {

        Geocoder geoCoder;

        public MainPage()
        {
            InitializeComponent();
            geoCoder = new Geocoder();
        }

        public async void FindOnMap(object sender, EventArgs e)
        {
            var from = fromLocation.Text;
            var to = toLocation.Text;

            if (String.IsNullOrEmpty(from))
            {
                ShowAlert("", "Please enter the Origin Location Address");
            }
            else if (String.IsNullOrEmpty(to))
            {
                ShowAlert("", "Please enter the Destination Location Address");
            }
            else
            {
                try
                {
                    geocodedOutputLabel.Text = "";
                    UserDialogs.Instance.ShowLoading("Locating");
                    var flocation = await geoCoder.GetPositionsForAddressAsync(from);
                    var tlocation = await geoCoder.GetPositionsForAddressAsync(to);

                    var fromloca = flocation.FirstOrDefault();
                    var tolocation = tlocation.FirstOrDefault();

                    if (fromloca.Latitude == 0 || fromloca.Longitude == 0)
                    {
                        ShowAlert("Location Not Found", String.Format("Location {0} not found ", from));
                        UserDialogs.Instance.HideLoading();
                    }
                    else if (fromloca.Latitude == 0 || fromloca.Longitude == 0)
                    {
                        ShowAlert("Location Not Found", String.Format("Location {0} not found ", to));
                        UserDialogs.Instance.HideLoading();
                    }
                    else
                    {
                        var dist = getDistanceFromLatLonInKm(fromloca.Latitude, fromloca.Longitude, tolocation.Latitude, tolocation.Longitude);

                        var newdistmap = new DistanceMap()
                        {
                            fromLatitude = fromloca.Latitude,
                            fromLongitude = fromloca.Longitude,
                            fromName = fromLocation.Text,
                            fromLocation = fromloca,
                            toLocation = tolocation,
                            toName = toLocation.Text,
                            toLatitude = tolocation.Latitude,
                            toLongitude = tolocation.Longitude,
                            Distance = dist
                        };
                        fromLocation.Text = "";
                        toLocation.Text = "";
                        await Navigation.PushModalAsync(new NavigationPage(new MapPage(newdistmap)));
                        UserDialogs.Instance.HideLoading();
                    }

                }
                catch (Exception x)
                {
                    //fromLocation.Text = "";
                    //toLocation.Text = "";
                    UserDialogs.Instance.HideLoading();
                    ShowAlert("Error", "An unexpected error occured! Please try again");
                }

            }
        }

        public async void ShowAlert(string title, string message)
        {
            await DisplayAlert(title, message, "Cancel");
        }

        private double getDistanceFromLatLonInKm(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; // Radius of the earth in km
            var dLat = deg2rad(lat2 - lat1);  // deg2rad below
            var dLon = deg2rad(lon2 - lon1);
            var a =
              Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
              Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) *
              Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c; // Distance in km
            return d;
        }

        private double deg2rad(double deg)
        {
            return deg * (Math.PI / 180);
        }


    }
}
