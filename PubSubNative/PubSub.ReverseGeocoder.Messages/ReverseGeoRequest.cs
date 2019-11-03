using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSub.ReverseGeocoder.Contracts
{
    public class ReverseGeoRequest
    {
        public float Lat { get; }
        public float Lon { get; }

        public ReverseGeoRequest(float lat, float lon)
        {
            Lat = lat;
            Lon = lon;
        }
    }
}
