using System.Collections.Generic;
using System.Threading.Tasks;
using AddressService.Core.Dto;

namespace AddressService.Handlers.BusinessLogic
{
    public interface INearestPostcodeGetter
    {
        Task<IReadOnlyList<NearestPostcodeDto>> GetNearestPostcodesAsync(string postcode, int? radiusInMetres = null, int? maxNumberOfResults = null);
    }
}