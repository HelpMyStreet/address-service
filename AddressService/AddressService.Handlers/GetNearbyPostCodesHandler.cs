using AddressService.Core.Config;
using AddressService.Core.Domains.Entities.Request;
using AddressService.Core.Domains.Entities.Response;
using AddressService.Core.Dto;
using AddressService.Core.Services.PostcodeIo;
using AddressService.Core.Utils;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.Handlers
{
    public class GetNearbyPostcodesHandler : IRequestHandler<GetNearbyPostcodesRequest, GetNearbyPostcodesResponse>
    {
        private readonly IPostcodeIoService _postcodeIoService;
        private readonly IMapper _mapper;
        private readonly IPostcodeGetter _postcodeGetter;
        private readonly IOptionsSnapshot<ApplicationConfig> _applicationConfig;

        public GetNearbyPostcodesHandler(IPostcodeIoService postcodeIoService, IMapper mapper, IPostcodeGetter postcodeGetter, IOptionsSnapshot<ApplicationConfig> applicationConfig)
        {
            _postcodeIoService = postcodeIoService;
            _mapper = mapper;
            _postcodeGetter = postcodeGetter;
            _applicationConfig = applicationConfig;
        }

        public async Task<GetNearbyPostcodesResponse> Handle(GetNearbyPostcodesRequest request, CancellationToken cancellationToken)
        {
            string postcode = PostcodeCleaner.CleanPostcode(request.Postcode);

            // call Postcode IO for nearest postcodes
            PostCodeIoNearestRootResponse postCodeIoResponse = await _postcodeIoService.GetNearbyPostCodesAsync(postcode, cancellationToken);

            IEnumerable<string> nearestPostcodes = postCodeIoResponse.Result.OrderBy(x => x.Distance)
                .Take(_applicationConfig.Value.NearestPostcodesLimit)
                .Select(x => PostcodeCleaner.CleanPostcode(x.Postcode)).ToList();

            // get postcodes
            IEnumerable<PostcodeDto> postcodeDtos = await _postcodeGetter.GetPostcodesAsync(nearestPostcodes, cancellationToken);

            // create response
            GetNearbyPostcodesResponse getNearbyPostcodesResponse = new GetNearbyPostcodesResponse();
            getNearbyPostcodesResponse.Postcodes = _mapper.Map<IEnumerable<PostcodeDto>, IEnumerable<PostcodeResponse>>(postcodeDtos);
            
            return getNearbyPostcodesResponse;
        }
    }
}
