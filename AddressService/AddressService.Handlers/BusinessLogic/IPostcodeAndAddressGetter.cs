using AddressService.Core.Dto;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.Handlers.BusinessLogic
{
    public interface IPostcodeAndAddressGetter
    {
        Task<PostcodeDto> GetPostcodeAsync(string postcode, CancellationToken cancellationToken);
        Task<IEnumerable<PostcodeDto>> GetPostcodesAsync(IEnumerable<string> postcodes, CancellationToken cancellationToken);

        Task<IEnumerable<PostcodeWithNumberOfAddressesDto>> GetNumberOfAddressesPerPostcodeAsync(IEnumerable<string> postcodes, CancellationToken cancellationToken);
    }
}