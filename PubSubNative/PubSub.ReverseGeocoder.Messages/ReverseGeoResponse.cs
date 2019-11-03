using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSub.ReverseGeocoder.Contracts
{
    public class ReverseGeoResponse
    {
        public string City { get; }

        public ReverseGeoResponse(string city)
        {
            City = city;
        }
    }
}
