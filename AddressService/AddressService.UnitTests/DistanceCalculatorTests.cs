using AddressService.Core.Utils;
using NUnit.Framework;
using System;

namespace AddressService.UnitTests
{
    public class DistanceCalculatorTests
    {
        [Test]

        public void ReturnCorrectDistance()
        {
            // latitudes, longitudes and distance taken from postcode IO (SQL Server agrees with the distance).  This function is not quite as accurate (hence the rounding).
            double result = DistanceCalculator.GetDistance(52.954885, -1.155263, 52.955491, -1.155413);

            Assert.AreEqual(Math.Round(68.18827704, 0), Math.Round(result, 0));
        }
    }
}
