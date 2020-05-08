using AddressService.Core.Contracts;
using AddressService.Core.Dto;
using AddressService.Handlers.BusinessLogic;
using AddressService.Handlers.Cache;
using HelpMyStreet.Utils.Extensions;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.Handlers
{
    public class GetNumberOfAddressesPerPostcodeInBoundaryHandler : IRequestHandler<GetNumberOfAddressesPerPostcodeInBoundaryRequest, GetNumberOfAddressesPerPostcodeInBoundaryResponse>
    {
        private readonly IPostcodeCache _postcodeCache;
        private readonly IPostcodeAndAddressGetter _postcodeAndAddressGetter;

        public GetNumberOfAddressesPerPostcodeInBoundaryHandler(IPostcodeCache postcodeCache, IPostcodeAndAddressGetter postcodeAndAddressGetter)
        {
            _postcodeCache = postcodeCache;
            _postcodeAndAddressGetter = postcodeAndAddressGetter;
        }

        public async Task<GetNumberOfAddressesPerPostcodeInBoundaryResponse> Handle(GetNumberOfAddressesPerPostcodeInBoundaryRequest request, CancellationToken cancellationToken)
        {
            IReadOnlyDictionary<string, PostcodeCoordinateDto> postcodesWithCoordinates = await _postcodeCache.GetAllPostcodeCoordinatesAsync();

            // slow... takes about 800-900ms to filter 1.7m postcodes on my machine
            IEnumerable<PostcodeCoordinateDto> postCodesWithinBoundary = postcodesWithCoordinates.Values.WhereWithinBoundary(request.SWLatitude, request.SWLongitude, request.NELatitude, request.NELongitude).ToList();

            IEnumerable<string> postcodes = postCodesWithinBoundary.Select(x => x.Postcode);

            IEnumerable<PostcodeWithNumberOfAddressesDto> postCodesWithNumberOfAddresses = await _postcodeAndAddressGetter.GetNumberOfAddressesPerPostcodeAsync(postcodes, cancellationToken);

            GetNumberOfAddressesPerPostcodeInBoundaryResponse getNumberOfAddressesPerPostcodeInBoundaryResponse = new GetNumberOfAddressesPerPostcodeInBoundaryResponse()
            {
                PostcodesWithNumberOfAddresses = postCodesWithNumberOfAddresses.Select(x => new PostcodeWithNumberOfAddresses()
                {
                    Postcode = x.Postcode,
                    NumberOfAddresses = x.NumberOfAddresses
                }).ToList()
            };

            return getNumberOfAddressesPerPostcodeInBoundaryResponse;
        }
    }
}
