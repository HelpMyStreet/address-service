using AddressService.Core.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AddressService.Handlers
{
    public interface INearestPostcodeGetter
    {
        Task<IReadOnlyList<NearestPostcodeDto>> GetNearestPostcodesAsync(string postcode, int? radiusInMetres = null, int? maxNumberOfResults = null);
    }
}