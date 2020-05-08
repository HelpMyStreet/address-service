using System.Collections.Generic;
using System.Threading.Tasks;
using AddressService.Core.Dto;

namespace AddressService.Handlers.BusinessLogic
{
    public interface IPostcodeCoordinatesGetter
    {
        Task<IReadOnlyDictionary<string, PostcodeCoordinateDto>> GetPostcodeCoordinatesAsync(IEnumerable<string> neededPostcodes);
    }
}