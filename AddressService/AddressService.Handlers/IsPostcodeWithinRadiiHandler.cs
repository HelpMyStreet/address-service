using AddressService.Core.Dto;
using AddressService.Core.Utils;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Utils.Utils;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.Handlers
{
    public class IsPostcodeWithinRadiiHandler : IRequestHandler<IsPostcodeWithinRadiiRequest, IsPostcodeWithinRadiiResponse>
    {
        private readonly IPostcodeCoordinatesGetter _postcodeCoordinatesGetter;

        public IsPostcodeWithinRadiiHandler(IPostcodeCoordinatesGetter postcodeCoordinatesGetter)
        {
            _postcodeCoordinatesGetter = postcodeCoordinatesGetter;
        }

        public async Task<IsPostcodeWithinRadiiResponse> Handle(IsPostcodeWithinRadiiRequest request, CancellationToken cancellationToken)
        {
            request.Postcode = PostcodeFormatter.FormatPostcode(request.Postcode);

            // not formatting postcodes for speed (they should be sent in in the correct format)
            HashSet<string> requiredPostcodes = request.PostcodeWithRadiuses.Select(x => x.Postcode).ToHashSet();
            requiredPostcodes.Add(request.Postcode);

            IReadOnlyDictionary<string, CoordinatesDto> postcodeCoordinates = await _postcodeCoordinatesGetter.GetPostcodeCoordinatesAsync(requiredPostcodes);

            // this shouldn't return null due to the postcode not being in the dictionary as it will have been validated at the beginning of the request
            postcodeCoordinates.TryGetValue(request.Postcode, out CoordinatesDto postcodeToCompareToLatitudeLongitude);

            List<int> idsInRadius = new List<int>(request.PostcodeWithRadiuses.Count / 100);

            foreach (PostcodeWithRadius p in request.PostcodeWithRadiuses)
            {
                if (postcodeCoordinates.TryGetValue(p.Postcode, out CoordinatesDto postcodeWithLatLong))
                {
                    var distanceInMetres = DistanceCalculator.GetDistance(postcodeToCompareToLatitudeLongitude.Latitude, postcodeToCompareToLatitudeLongitude.Longitude, postcodeWithLatLong.Latitude, postcodeWithLatLong.Longitude);
                    bool isWithinRadius = distanceInMetres <= p.RadiusInMetres;

                    if (isWithinRadius)
                    {
                        idsInRadius.Add(p.Id);
                    }
                }
            }

            IsPostcodeWithinRadiiResponse isPostcodeWithinRadiiResponse = new IsPostcodeWithinRadiiResponse()
            {
                IdsWithinRadius = idsInRadius
            };
            
            return isPostcodeWithinRadiiResponse;
        }


    }

}
