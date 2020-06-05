using AddressService.Core.Dto;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using AddressService.Core.Utils;
using AddressService.Handlers.BusinessLogic;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Utils.Utils;

namespace AddressService.Handlers
{
    public class GetPostcodeHandler : IRequestHandler<GetPostcodeRequest, GetPostcodeResponse>
    {
        private readonly IMapper _mapper;
        private readonly IPostcodeAndAddressGetter _postcodeAndAddressGetter;
        private readonly IAddressDetailsSorter _addressDetailsSorter;

        public GetPostcodeHandler(IMapper mapper, IPostcodeAndAddressGetter postcodeAndAddressGetter, IAddressDetailsSorter addressDetailsSorter)
        {
            _mapper = mapper;
            _postcodeAndAddressGetter = postcodeAndAddressGetter;
            _addressDetailsSorter = addressDetailsSorter;
        }

        public async Task<GetPostcodeResponse> Handle(GetPostcodeRequest request, CancellationToken cancellationToken)
        {
            request.Postcode = PostcodeFormatter.FormatPostcode(request.Postcode);

            PostcodeDto postcodeDto = await _postcodeAndAddressGetter.GetPostcodeAsync(request.Postcode, cancellationToken);

            GetPostcodeResponse getNearbyGetPostcodesResponse = _mapper.Map<PostcodeDto, GetPostcodeResponse>(postcodeDto);
            getNearbyGetPostcodesResponse.AddressDetails = _addressDetailsSorter.OrderAddressDetailsResponse(getNearbyGetPostcodesResponse.AddressDetails);
            return getNearbyGetPostcodesResponse;
        }
    }
}
