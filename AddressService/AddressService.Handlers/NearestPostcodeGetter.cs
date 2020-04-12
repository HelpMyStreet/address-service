using AddressService.Core.Config;
using AddressService.Core.Dto;
using AddressService.Core.Interfaces.Repositories;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AddressService.Handlers
{
    public class NearestPostcodeGetter : INearestPostcodeGetter
    {
        private readonly IRepository _repository;
        private readonly IOptionsSnapshot<ApplicationConfig> _applicationConfig;

        public NearestPostcodeGetter(IRepository repository, IOptionsSnapshot<ApplicationConfig> applicationConfig)
        {
            _repository = repository;
            _applicationConfig = applicationConfig;
        }

        /// <summary>
        /// Get nearest postcodes sorted by distance
        /// </summary>
        /// <param name="postcode">Postcode</param>
        /// <param name="radiusInMetres">Radius in miles (optional)/param>
        /// <param name="maxNumberOfResults">Max number of results (optional)</param>
        /// <returns></returns>
        public async Task<IReadOnlyList<NearestPostcodeDto>> GetNearestPostcodesAsync(string postcode, int? radiusInMetres = null, int? maxNumberOfResults = null)
        {
            if (radiusInMetres == null)
            {
                radiusInMetres = _applicationConfig.Value.DefaultNearestPostcodeRadiusInMetres;
            }

            if (maxNumberOfResults == null)
            {
                maxNumberOfResults = _applicationConfig.Value.DefaultMaxNumberOfNearbyPostcodes;
            }

            IEnumerable<NearestPostcodeDto> nearestPostCodes = await _repository.GetNearestPostcodesAsync(postcode, (double)radiusInMetres, (int)maxNumberOfResults);
           
            List<NearestPostcodeDto> orderedNearestPostCodes = nearestPostCodes.OrderBy(x => x.DistanceInMetres).Take((int)maxNumberOfResults).ToList();
        
            return orderedNearestPostCodes;
        }
    }
}
