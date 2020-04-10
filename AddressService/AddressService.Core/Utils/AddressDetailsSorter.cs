using HelpMyStreet.Contracts.AddressService.Response;
using System.Collections.Generic;
using System.Linq;

namespace AddressService.Core.Utils
{
    public class AddressDetailsSorter : IAddressDetailsSorter
    {
        public IReadOnlyList<AddressDetailsResponse> OrderAddressDetailsResponse(IEnumerable<AddressDetailsResponse> addressDetails)
        {
            //Locality should never have numbers anyway...
            var Comparer = new AddressComparer();
            return addressDetails.OrderBy(x => x.Locality)
                .ThenBy(x => x.AddressLine3, Comparer)
                .ThenBy(x => x.AddressLine2, Comparer)
                .ThenBy(x => x.AddressLine1, Comparer)
                .ToList();
        }
    }
}
