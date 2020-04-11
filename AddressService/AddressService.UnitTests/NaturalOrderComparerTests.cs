using AddressService.Core.Utils;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace AddressService.UnitTests
{
    public class NaturalOrderComparerTests
    {

        [Test]
        public void Test_Letters()
        {
            List<string> someStrings = new List<string>()
            {
                "b", "a", "d", "c","bb"
            };

            List<string> result = someStrings.OrderBy(x => x).ToList();

            IOrderedEnumerable<string> sorted = result.OrderBy(x => x);
            CollectionAssert.AreEqual(sorted.ToList(), result.ToList());

        }

        [Test]
        public void Test_Numbers()
        {
            List<string> someStrings = new List<string>()
            {
                "2", "20", "1", "10", "4", "40", "3", "30"
            };

            List<string> result = someStrings.OrderBy(x => x, new NaturalOrderComparer()).ToList();

            Assert.AreEqual(result[0], "1");
            Assert.AreEqual(result[1], "2");
            Assert.AreEqual(result[2], "3");
            Assert.AreEqual(result[3], "4");
            Assert.AreEqual(result[4], "10");
            Assert.AreEqual(result[5], "20");
            Assert.AreEqual(result[6], "30");
            Assert.AreEqual(result[7], "40");
        }

        [Test]
        public void Test_LettersAndNumbers()
        {
            List<string> someStrings = new List<string>()
            {
                "10", "1", "a", "b"
            };

            List<string> result = someStrings.OrderBy(x => x, new NaturalOrderComparer()).ToList();

            Assert.AreEqual(result[0], "1");
            Assert.AreEqual(result[1], "10");
            Assert.AreEqual(result[2], "a");
            Assert.AreEqual(result[3], "b");
        }


        [Test]
        public void Test_WithNulls()
        {
            List<string> someStrings = new List<string>()
            {
                "10", null, "a"
            };

            List<string> result = someStrings.OrderBy(x => x, new NaturalOrderComparer()).ToList();

            Assert.AreEqual(result[0], null);
            Assert.AreEqual(result[1], "10");
            Assert.AreEqual(result[2], "a");
        }
    }
}
