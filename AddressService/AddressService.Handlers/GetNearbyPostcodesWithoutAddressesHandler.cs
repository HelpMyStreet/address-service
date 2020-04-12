using AddressService.Core.Contracts;
using AddressService.Core.Dto;
using AutoMapper;
using HelpMyStreet.Utils.Utils;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.Handlers
{
    public class GetNearbyPostcodesWithoutAddressesHandler : IRequestHandler<GetNearbyPostcodesWithoutAddressesRequest, GetNearbyPostcodesWithoutAddressesResponse>
    {
        private readonly IMapper _mapper;
        private readonly INearestPostcodeGetter _nearestPostcodeGetter;

        public GetNearbyPostcodesWithoutAddressesHandler(IMapper mapper, INearestPostcodeGetter nearestPostcodeGetter)
        {
            _mapper = mapper;
            _nearestPostcodeGetter = nearestPostcodeGetter;
        }

        public async Task<GetNearbyPostcodesWithoutAddressesResponse> Handle(GetNearbyPostcodesWithoutAddressesRequest request, CancellationToken cancellationToken)
        {
            request.Postcode = PostcodeFormatter.FormatPostcode(request.Postcode);

            IEnumerable<NearestPostcodeDto> nearestPostcodeDtos = await _nearestPostcodeGetter.GetNearestPostcodesAsync(request.Postcode, request.RadiusInMetres, request.MaxNumberOfResults);
            
            IEnumerable<NearestPostcode> nearestPostcodes = _mapper.Map<IEnumerable<NearestPostcodeDto>, IEnumerable<NearestPostcode>>(nearestPostcodeDtos);

            GetNearbyPostcodesWithoutAddressesResponse getNearbyPostcodesWithoutAddressesResponse = new GetNearbyPostcodesWithoutAddressesResponse()
            {
                NearestPostcodes = nearestPostcodes.ToList()
            };

            return getNearbyPostcodesWithoutAddressesResponse;
        }
    }
}
