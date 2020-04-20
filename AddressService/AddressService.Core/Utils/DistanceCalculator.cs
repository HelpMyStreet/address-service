using System;

namespace AddressService.Core.Utils
{
    public class DistanceCalculator
    {
        public static double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            int r = 6371; // Radius of the earth in km
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);
            double a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = r * c * 1000; // Distance in m
            return d;
        }

        private static double ToRadians(double deg)
        {
            return deg * (Math.PI / 180);
        }
    }
}
