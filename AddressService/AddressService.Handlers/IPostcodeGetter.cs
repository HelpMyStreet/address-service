using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AddressService.Core.Dto;

namespace AddressService.Handlers
{
    public interface IPostcodeGetter
    {
        Task<PostcodeDto> GetPostcodeAsync(string postcode, CancellationToken cancellationToken);
        Task<IEnumerable<PostcodeDto>> GetPostcodesAsync(IEnumerable<string> postcodes, CancellationToken cancellationToken);
    }
}