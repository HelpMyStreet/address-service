using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AddressService.Core.Dto;

namespace AddressService.Handlers.BusinessLogic
{
    public interface IQasAddressGetter
    {
        Task<IEnumerable<PostcodeDto>> GetPostCodesAndAddressesFromQasAsync(IEnumerable<string> missingPostcodes, CancellationToken cancellationToken);
    }
}