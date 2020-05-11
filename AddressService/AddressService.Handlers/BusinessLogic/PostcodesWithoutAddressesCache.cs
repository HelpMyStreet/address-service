using System.Collections.Generic;

namespace AddressService.Handlers.BusinessLogic
{
    public class PostcodesWithoutAddressesCache : IPostcodesWithoutAddressesCache
    {
        private static readonly HashSet<string> _postcodesWithoutAddressesCache = new HashSet<string>();

        public IEnumerable<string> PostcodesWithoutAddresses => _postcodesWithoutAddressesCache;

        public void AddRange(IEnumerable<string> postcodes)
        {
            foreach (var postcode in postcodes)
            {
                _postcodesWithoutAddressesCache.Add(postcode);
            }
        }
    }
}
