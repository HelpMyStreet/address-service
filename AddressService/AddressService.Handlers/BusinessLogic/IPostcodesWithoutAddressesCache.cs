using System.Collections.Generic;

namespace AddressService.Handlers.BusinessLogic
{
    public interface IPostcodesWithoutAddressesCache
    {
        IEnumerable<string> PostcodesWithoutAddresses { get; }

        void AddRange(IEnumerable<string> postcode);
    }
}