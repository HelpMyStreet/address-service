using AddressService.Core.Utils;
using HelpMyStreet.Contracts.AddressService.Response;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace AddressService.UnitTests
{
    public class AddressDetailsSorterTests
    {
        [Test]
        public void LocalityIsSortedCorrectly()
        {

            IEnumerable<AddressDetailsResponse> addressDetails = new List<AddressDetailsResponse>()
            {
                new AddressDetailsResponse()
                {
                    AddressLine1 = "aaa",
                    AddressLine2 = "aaa",
                    AddressLine3 = null,
                    Locality = "ccc"
                },
                new AddressDetailsResponse()
                {
                    AddressLine1 = "aaa",
                    AddressLine2 = "aaa",
                    AddressLine3 = null,
                    Locality = "aaa"
                },
                new AddressDetailsResponse()
                {
                    AddressLine1 = "aaa",
                    AddressLine2 = "aaa",
                    AddressLine3 = null,
                    Locality = "bbb"
                },
            };

            AddressDetailsSorter addressDetailsSorter = new AddressDetailsSorter();
            IReadOnlyList<AddressDetailsResponse> result = addressDetailsSorter.OrderAddressDetailsResponse(addressDetails);

            IOrderedEnumerable<AddressDetailsResponse> sorted = result.OrderBy(x => x.Locality);
            CollectionAssert.AreEqual(sorted.ToList(), result.ToList());
        }

        [Test]
        public void AddressLine1IsSortedCorrectly()
        {
            IEnumerable<AddressDetailsResponse> addressDetails = new List<AddressDetailsResponse>()
            {
                new AddressDetailsResponse()
                {
                    AddressLine1 = "ccc",
                    AddressLine2 = "aaa",
                    AddressLine3 = "aaa",
                    Locality = null
                },
                new AddressDetailsResponse()
                {
                    AddressLine1 = "aaa",
                    AddressLine2 = "aaa",
                    AddressLine3 ="aaa",
                    Locality = null
                },
                new AddressDetailsResponse()
                {
                    AddressLine1 = "bbb",
                    AddressLine2 = "aaa",
                    AddressLine3 ="aaa",
                    Locality = null
                },
            };

            AddressDetailsSorter addressDetailsSorter = new AddressDetailsSorter();
            IReadOnlyList<AddressDetailsResponse> result = addressDetailsSorter.OrderAddressDetailsResponse(addressDetails);

            IOrderedEnumerable<AddressDetailsResponse> sorted = result.OrderBy(x => x.AddressLine1);
            CollectionAssert.AreEqual(sorted.ToList(), result.ToList());
        }

        [Test]
        public void AddressLine2IsSortedCorrectly()
        {
            IEnumerable<AddressDetailsResponse> addressDetails = new List<AddressDetailsResponse>()
            {
                new AddressDetailsResponse()
                {
                    AddressLine1 = null,
                    AddressLine2 = "bbb",
                    AddressLine3 = "aaa",
                    Locality = "aaa"
                },
                new AddressDetailsResponse()
                {
                    AddressLine1 = null,
                    AddressLine2 = "ccc",
                    AddressLine3 = "aaa",
                    Locality = "aaa"
                },
                new AddressDetailsResponse()
                {
                    AddressLine1 = null,
                    AddressLine2 = "aaa",
                    AddressLine3 ="aaa",
                    Locality = "aaa"
                },
            };

            AddressDetailsSorter addressDetailsSorter = new AddressDetailsSorter();
            IReadOnlyList<AddressDetailsResponse> result = addressDetailsSorter.OrderAddressDetailsResponse(addressDetails);

            IOrderedEnumerable<AddressDetailsResponse> sorted = result.OrderBy(x => x.AddressLine2);
            CollectionAssert.AreEqual(sorted.ToList(), result.ToList());
        }


        [Test]
        public void AddressLine3IsSortedCorrectly()
        {
            IEnumerable<AddressDetailsResponse> addressDetails = new List<AddressDetailsResponse>()
            {
                new AddressDetailsResponse()
                {
                    AddressLine1 = "aaa",
                    AddressLine2 = null,
                    AddressLine3 = "bbb",
                    Locality = "aaa"
                },
                new AddressDetailsResponse()
                {
                    AddressLine1 = "aaa",
                    AddressLine2 = null,
                    AddressLine3 = "ccc",
                    Locality = "aaa"
                },
                new AddressDetailsResponse()
                {
                    AddressLine1 = "aaa",
                    AddressLine2 = null,
                    AddressLine3 ="aaa",
                    Locality = "aaa"
                },
            };

            AddressDetailsSorter addressDetailsSorter = new AddressDetailsSorter();
            IReadOnlyList<AddressDetailsResponse> result = addressDetailsSorter.OrderAddressDetailsResponse(addressDetails);

            IOrderedEnumerable<AddressDetailsResponse> sorted = result.OrderBy(x => x.AddressLine3);
            CollectionAssert.AreEqual(sorted.ToList(), result.ToList());
        }
    }
}
