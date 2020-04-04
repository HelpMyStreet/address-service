using System;
using AddressService.Core.Utils;
using NUnit.Framework;

namespace AddressService.UnitTests
{
    public class PostcodeFormatterTests
    {

        [TestCase("NG1 5FS", "NG1 5FS")]
        [TestCase("ng15fs", "NG1 5FS")]
        [TestCase("ng1 5fs", "NG1 5FS")]
        [TestCase("ng1 5fs ", "NG1 5FS")]
        [TestCase(" ng1 5fs ", "NG1 5FS")]
        [TestCase(" ng1 5fs ", "NG1 5FS")]
        [TestCase("  ng1  5fs  ", "NG1 5FS")]

        [TestCase("M11AA", "M1 1AA")]
        [TestCase("M601NW", "M60 1NW")]
        [TestCase("CR26XH", "CR2 6XH")]
        [TestCase("DN551PT", "DN55 1PT")]
        [TestCase("W1A1HQ", "W1A 1HQ")]
        [TestCase("EC1A1BB", "EC1A 1BB")]

        [TestCase("   N5FS  ", "N5FS", Description = "Postcode too short to add space (invalid postcode)")]
        [TestCase("N5FS", "N5FS", Description = "Postcode too short to add space (invalid postcode)")]

        [TestCase("   NG15A5FS  ", "NG15A5FS", Description = "Postcode too long to add space (invalid postcode)")]
        [TestCase("NG15A5FS", "NG15A5FS", Description = "Postcode too long to add space (invalid postcode)")]


        [TestCase("  ", "  ")]
        [TestCase("", "")]

        public void CleanPostcode(string postcodeToTest, string expected)
        {
            string postcodeInput = String.Copy(postcodeToTest);
            string result = PostcodeFormatter.FormatPostcode(postcodeInput);
            Assert.AreEqual(expected, result);

            // check input was not modified
            Assert.AreEqual(postcodeToTest, postcodeInput);
            Assert.IsFalse(Object.ReferenceEquals(result, postcodeToTest));
        }

        [Test]
        public void ThrowExceptionOnNullInput()
        {
            Assert.Throws<ArgumentNullException>(() => PostcodeFormatter.FormatPostcode(null));
        }

    }
}
