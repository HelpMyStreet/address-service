using AddressService.PostcodeLoader;
using NUnit.Framework;
using System;

namespace AddressService.UnitTests
{
    public class OnsActivePostcodeDeterminerTests
    {
        // valid input
        [TestCase("\"199001\"", "\"\"", true)]
        [TestCase("\"201010\"", "\"200010\"", true)]
        [TestCase("\"199001\"", "\"200010\"", false)]

        // invalid input
        [TestCase("\"199001\"", "", true)]
        [TestCase("", "\"199001\"", true)]
        [TestCase("", "", true)]

        [TestCase("\"199001\"", null, true)]
        [TestCase(null, "\"199001\"", true)]
        [TestCase(null, null, true)]

        [TestCase("\"199001\"", "invalidDate", true)]
        [TestCase("invalidDate", "\"199001\"", true)]
        [TestCase("invalidDate", "invalidDate", true)]

        public void IsPostcodeActive(string introDate, string terminationDate, bool expectedResult)
        {
            var dateNow = new DateTime(2020,04,12);
            var result = OnsActivePostcodeDeterminer.IsPostcodeActive(introDate, terminationDate, dateNow);

            Assert.AreEqual(result,expectedResult);

        }
    }
}
