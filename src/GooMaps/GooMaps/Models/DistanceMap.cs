using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.GoogleMaps;

namespace GooMaps.Models
{
    public class DistanceMap
    {
        public double fromLatitude { get; set; }
        public double fromLongitude { get; set; }
        public Position fromLocation { get; set; }
        public string fromName { get; set; }
        public double toLatitude { get; set; }
        public double toLongitude { get; set; }
        public Position toLocation { get; set; }
        public string toName { get; set; }
        public double Distance { get; set; }
    }
}
