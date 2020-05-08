using AddressService.Core.Dto;
using AddressService.Handlers.Cache;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AddressService.Handlers.BusinessLogic
{
    public class PostcodeCoordinatesGetter : IPostcodeCoordinatesGetter
    {
        private readonly IPostcodeCache _postcodeCache;

        public PostcodeCoordinatesGetter(IPostcodeCache postcodeCache)
        {
            _postcodeCache = postcodeCache;
        }

        public async Task<IReadOnlyDictionary<string, PostcodeCoordinateDto>> GetPostcodeCoordinatesAsync(IEnumerable<string> neededPostcodes)
        {
            IReadOnlyDictionary<string, PostcodeCoordinateDto> postcodeCoordinates = await _postcodeCache.GetAllPostcodeCoordinatesAsync();

            Dictionary<string, PostcodeCoordinateDto> requiredPostcodes = new Dictionary<string, PostcodeCoordinateDto>();

            foreach (string neededPostcode in neededPostcodes)
            {
                if (postcodeCoordinates.TryGetValue(neededPostcode, out var coordinatesDto))
                {
                    requiredPostcodes[neededPostcode] = coordinatesDto;
                }
            }

            return requiredPostcodes;
        }
    }
}
