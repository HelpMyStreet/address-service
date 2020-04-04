using AddressService.Core.Domains.Entities.Request;
using AddressService.Core.Domains.Entities.Response;
using AddressService.Core.Dto;
using AddressService.Core.Utils;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.Handlers
{
    public class GetPostcodeHandler : IRequestHandler<GetPostcodeRequest, GetPostcodeResponse>
    {
        private readonly IMapper _mapper;
        private readonly IPostcodeGetter _postcodeGetter;

        public GetPostcodeHandler(IMapper mapper, IPostcodeGetter postcodeGetter)
        {
            _mapper = mapper;
            _postcodeGetter = postcodeGetter;
        }

        public async Task<GetPostcodeResponse> Handle(GetPostcodeRequest request, CancellationToken cancellationToken)
        {
            request.Postcode = PostcodeFormatter.FormatPostcode(request.Postcode);

            PostcodeDto postcodeDto = await _postcodeGetter.GetPostcodeAsync(request.Postcode, cancellationToken);

            GetPostcodeResponse getNearbyGetPostcodesResponse = _mapper.Map<PostcodeDto, GetPostcodeResponse>(postcodeDto);

            return getNearbyGetPostcodesResponse;
        }
    }
}
