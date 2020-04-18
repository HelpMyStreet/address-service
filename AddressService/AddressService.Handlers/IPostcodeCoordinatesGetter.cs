using System.Collections.Generic;
using System.Threading.Tasks;
using AddressService.Core.Dto;

namespace AddressService.Handlers
{
    public interface IPostcodeCoordinatesGetter
    {
        Task<IReadOnlyDictionary<string, CoordinatesDto>> GetPostcodeCoordinatesAsync(IEnumerable<string> neededPostcodes);
    }
}