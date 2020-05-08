using System.Collections.Generic;
using System.Threading.Tasks;
using AddressService.Core.Dto;

namespace AddressService.Handlers.Cache
{
    public interface IPostcodeCache
    {
        Task<IReadOnlyDictionary<string, PostcodeCoordinateDto>> GetAllPostcodeCoordinatesAsync();
    }
}