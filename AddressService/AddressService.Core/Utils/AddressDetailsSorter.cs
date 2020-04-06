using HelpMyStreet.Contracts.AddressService.Response;
using System.Collections.Generic;
using System.Linq;

namespace AddressService.Core.Utils
{
    public class AddressDetailsSorter : IAddressDetailsSorter
    {
        public IReadOnlyList<AddressDetailsResponse> OrderAddressDetailsResponse(IEnumerable<AddressDetailsResponse> addressDetails)
        {
            return addressDetails.OrderBy(x => x.Locality)
                .ThenBy(x => x.AddressLine3)
                .ThenBy(x => x.AddressLine2)
                .ThenBy(x => x.AddressLine1)
                .ToList();
        }
    }
}
