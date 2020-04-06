using System.Collections.Generic;
using HelpMyStreet.Contracts.AddressService.Response;

namespace AddressService.Core.Utils
{
    public interface IAddressDetailsSorter
    {
        IReadOnlyList<AddressDetailsResponse> OrderAddressDetailsResponse(IEnumerable<AddressDetailsResponse> addressDetails);
    }
}