using AddressService.Core.Contracts;
using AddressService.Core.Dto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.Handlers
{
    public class GetPostcodeCoordinatesHandler : IRequestHandler<GetPostcodeCoordinatesRequest, GetPostcodeCoordinatesResponse>
    {
        private readonly IPostcodeCoordinatesGetter _postcodeCoordinatesGetter;
        public GetPostcodeCoordinatesHandler(IPostcodeCoordinatesGetter postcodeCoordinatesGetter)
        {
            _postcodeCoordinatesGetter = postcodeCoordinatesGetter;
        }

        public async Task<GetPostcodeCoordinatesResponse> Handle(GetPostcodeCoordinatesRequest request, CancellationToken cancellationToken)
        {
            IEnumerable<string> requiredPostcodes = request.Postcodes.Where(x => !String.IsNullOrWhiteSpace(x));

            IReadOnlyDictionary<string, CoordinatesDto> postcodeCoordinateDtos = await _postcodeCoordinatesGetter.GetPostcodeCoordinatesAsync(requiredPostcodes);
            
            IEnumerable<PostcodeCoordinate> postcodeCoordinates = postcodeCoordinateDtos.Select(x => new PostcodeCoordinate()
            {
                Postcode = x.Key,
                Latitude = x.Value.Latitude,
                Longitude = x.Value.Longitude
            }).ToList();

            GetPostcodeCoordinatesResponse getPostcodeCoordinatesResponse = new GetPostcodeCoordinatesResponse()
            {
                PostcodeCoordinates = postcodeCoordinates
            };

            return getPostcodeCoordinatesResponse;
        }
    }
}
