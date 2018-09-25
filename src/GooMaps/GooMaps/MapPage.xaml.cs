using GooMaps.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms.GoogleMaps;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Android.Gms.Maps.Model;

namespace GooMaps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        private readonly Map map;

        public MapPage(DistanceMap input)
        {

            //InitializeComponent ();
            Title = "Location";
            NavigationPage.SetHasBackButton(this, true);
            map = new Map();

            var fromPin = new Pin
            {
                Type = PinType.Generic,
                Position = input.fromLocation,
                Label = input.fromName
            };

            var toPin = new Pin
            {
                Type = PinType.Generic,
                Position = input.toLocation,
                Label = input.toName
            };

            var arr = GetWayPoints(input.fromLocation, input.toLocation);
            var polypoints = DecodePolyline(arr);

            map.Pins.Add(fromPin);
            map.Pins.Add(toPin);

            //foreach (var position in polypoints)
            //{
            //    polylineOptions.Add(new LatLng(position.Latitude, position.Longitude));
            //}


            //var mid = GetMidPoint(input.fromLocation, input.toLocation);
            //map.MoveToRegion(new MapSpan(new Position(mid.Latitude, mid.Longitude), 0.05, 0.05));

            map.MoveToRegion(MapSpan.FromCenterAndRadius(input.fromLocation, Distance.FromMiles(Convert.ToInt32(input.Distance))));
            

            Content = new StackLayout
            {
                Padding = new Thickness(20, 20, 20, 20),
                Children =
                {
                    new Label
                    {
                        Text = String.Format("The distance from {0} to {1} is {2}km", input.fromName, input.toName, Math.Round(input.Distance, 2)),
                        TextColor = Color.Red,
                        HorizontalOptions = LayoutOptions.Center,
                        Margin = new Thickness(0, 0, 0 , 20)

                    },
                    map
                }
            };
        }

        public async void ShowAlert(string title, string message)
        {
            await DisplayAlert(title, message, "OK");
        }

        public static string GetWayPoints(Position origin, Position destination)
        {
            try
            {
                System.Net.WebRequest req = System.Net.WebRequest.Create(String.Format("https://maps.googleapis.com/maps/api/directions/json?origin={0},{1}&destination={2},{3}&key=AIzaSyBNvVBDOXXzwWHO4OHVwSu2C6NaNu0oeDU", origin.Latitude, origin.Longitude, destination.Latitude, destination.Longitude));
                System.Net.WebResponse resp = req.GetResponse();
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                return sr.ReadToEnd().Trim();
            }
            catch (Exception x)
            {
                throw x;
            }

        }

        public static List<Position> DecodePolyline(string encodedPoints)
        {
            if (string.IsNullOrEmpty(encodedPoints))
                throw new ArgumentNullException("encodedPoints");

            char[] polylineChars = encodedPoints.ToCharArray();
            int index = 0;

            int currentLat = 0;
            int currentLng = 0;
            int next5bits;
            int sum;
            int shifter;

            List<Position> polylinesPosition = new List<Position>();

            while (index < polylineChars.Length)
            {
                // calculate next latitude
                sum = 0;
                shifter = 0;
                do
                {
                    next5bits = (int)polylineChars[index++] - 63;
                    sum |= (next5bits & 31) << shifter;
                    shifter += 5;
                } while (next5bits >= 32 && index < polylineChars.Length);

                if (index >= polylineChars.Length)
                    break;

                currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                //calculate next longitude
                sum = 0;
                shifter = 0;
                do
                {
                    next5bits = (int)polylineChars[index++] - 63;
                    sum |= (next5bits & 31) << shifter;
                    shifter += 5;
                } while (next5bits >= 32 && index < polylineChars.Length);

                if (index >= polylineChars.Length && next5bits >= 32)
                    break;

                currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                polylinesPosition.Add(new Position(Convert.ToDouble(currentLat) / 1E5, Convert.ToDouble(currentLng) / 1E5));
            }

            return (polylinesPosition);
        }


    }
}