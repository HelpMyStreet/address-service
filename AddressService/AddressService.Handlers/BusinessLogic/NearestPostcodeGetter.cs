using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AddressService.Core.Config;
using AddressService.Core.Dto;
using AddressService.Core.Interfaces.Repositories;
using Microsoft.Extensions.Options;

namespace AddressService.Handlers.BusinessLogic
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

            int maxRadiusInMetres = 16094; // 10 miles
            if (radiusInMetres == null)
            {
                radiusInMetres = _applicationConfig.Value.DefaultNearestPostcodeRadiusInMetres;
            }

            if (radiusInMetres > 16094 || radiusInMetres < 1)
            {
                radiusInMetres = maxRadiusInMetres;
            }

            if (maxNumberOfResults == null)
            {
                maxNumberOfResults = _applicationConfig.Value.DefaultMaxNumberOfNearbyPostcodes;
            }

            IEnumerable<NearestPostcodeDto> nearestPostCodes;
            PreComputedNearestPostcodesDto preComputedNearestPostcodes;

            preComputedNearestPostcodes = await _repository.GetPreComputedNearestPostcodes(postcode);

            if (preComputedNearestPostcodes != null)
            {
                nearestPostCodes = NearestPostcodeCompressor.DecompressPreComputedPostcodes(preComputedNearestPostcodes);
            }
            else
            {
                nearestPostCodes = await _repository.GetNearestPostcodesAsync(postcode, (double)maxRadiusInMetres);
                preComputedNearestPostcodes = NearestPostcodeCompressor.CompressNearestPostcodeDtos(postcode, nearestPostCodes);
                await _repository.SavePreComputedNearestPostcodes(preComputedNearestPostcodes);
            }
    

            List<NearestPostcodeDto> orderedNearestPostCodes = nearestPostCodes.Where(x => x.DistanceInMetres <= radiusInMetres)
                .OrderBy(x => x.DistanceInMetres)
                .Take((int)maxNumberOfResults)
                .ToList();

            return orderedNearestPostCodes;
        }
    }
}
