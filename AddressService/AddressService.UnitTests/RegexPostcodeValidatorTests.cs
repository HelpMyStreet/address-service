using AddressService.Core.Validation;
using NUnit.Framework;

namespace AddressService.UnitTests
{
    public class RegexPostcodeValidatorTests
    {

        // Fails regex
        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase("  ", false)]
        [TestCase("NGG15FS", false)]
        [TestCase("NG15FSS", false)]
        [TestCase("NG15F", false)]

        [TestCase("NPT 6DZ", false)] // NPT was retired in 1984
        [TestCase("GIR 0AA", false)] // owned by Santander (previously Girobank) and doesn't relate to a geographic area
        [TestCase("NPT6DZ", false)] 
        [TestCase("GIR0AA", false)]

        // Passes regex
        [TestCase("AA9A 9AA", true)]
        [TestCase("A9A 9AA", true)]
        [TestCase("A9 9AA", true)]
        [TestCase("A99 9AA", true)]
        [TestCase("AA9 9AA", true)]
        [TestCase("AA99 9AA", true)]

        [TestCase("AA9A9AA", true)]
        [TestCase("A9A9AA", true)]
        [TestCase("A99AA", true)]
        [TestCase("A999AA", true)]
        [TestCase("AA99AA", true)]
        [TestCase("AA999AA", true)]

        [TestCase("aa9a 9aa", true)]
        [TestCase("a9a 9aa", true)]
        [TestCase("a9 9aa", true)]
        [TestCase("a99 9aa", true)]
        [TestCase("aa9 9aa", true)]
        [TestCase("aa99 9aa", true)]

        [TestCase("aa9a9aa", true)]
        [TestCase("a9a9aa", true)]
        [TestCase("a99aa", true)]
        [TestCase("a999aa", true)]
        [TestCase("aa99aa", true)]
        [TestCase("aa999aa", true)]
        public void IsPostcodeValid(string postcode, bool expectedResult)
        {
            RegexPostcodeValidator regexPostcodeValidator = new RegexPostcodeValidator();
            bool result = regexPostcodeValidator.IsPostcodeValid(postcode);

            Assert.AreEqual(expectedResult, result);

        }
    }
}
