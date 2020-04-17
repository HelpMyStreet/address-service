using AddressService.Core.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AddressService.Core.Interfaces.Repositories
{
    public interface IRepository
    {
        Task<IEnumerable<PostcodeDto>> GetPostcodesAsync(IEnumerable<string> postcodes);
        Task SaveAddressesAndFriendlyNameAsync(IEnumerable<PostcodeDto> postCodes);
        Task<bool> IsPostcodeInDbAndActive(string postcode);
        Task<IEnumerable<NearestPostcodeDto>> GetNearestPostcodesAsync(string postcode, double distanceInMetres);
        Task SavePreComputedNearestPostcodes(PreComputedNearestPostcodesDto preComputedNearestPostcodesDto);
        Task<PreComputedNearestPostcodesDto> GetPreComputedNearestPostcodes(string postcode);
        
        Task<IEnumerable<PostcodeWithCoordinatesDto>> GetPostcodeCoordinatesAsync(IEnumerable<string> postcodes);
    }
}
