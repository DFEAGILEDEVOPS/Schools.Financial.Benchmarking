using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFB.Web.Domain.Models
{
    public static class GeoCodeCalc
    {
        public static double CalcDistance(string lat1, string lng1, string lat2, string lng2)
        {
            var lat1Parsed = double.Parse(lat1);
            var lng1Parsed = double.Parse(lng1);
            var lat2Parsed = double.Parse(lat2);
            var lng2Parsed = double.Parse(lng2);

            var p1 = new GeoCoordinate(lat2Parsed, lng2Parsed);
            var p2 = new GeoCoordinate(lat1Parsed, lng1Parsed);

            return Math.Round(p1.GetDistanceTo(p2), 2);
        }
    }
}
