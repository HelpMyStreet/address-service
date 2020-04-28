using AddressService.Core.Dto;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using AddressService.Core.Utils;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Utils.Utils;
using System.Linq;
using AddressService.Core.Interfaces.Repositories;

namespace AddressService.Handlers
{
    public class GetPostcodesHandler : IRequestHandler<GetPostcodesRequest, GetPostcodesResponse>
    {
        private readonly IMapper _mapper;
        private readonly IRepository _repository;

        public GetPostcodesHandler(IMapper mapper, IRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<GetPostcodesResponse> Handle(GetPostcodesRequest request, CancellationToken cancellationToken)
        {
            GetPostcodesResponse getPostcodesResponse = new GetPostcodesResponse()
            {
                PostcodesResponse = new System.Collections.Generic.Dictionary<string, GetPostcodeResponse>()
            };
            request.PostcodeList.Postcodes = request.PostcodeList.Postcodes.Select(x => PostcodeFormatter.FormatPostcode(x)).ToList();
            System.Collections.Generic.IEnumerable<PostcodeDto> postcodesDto = await _repository.GetPostcodesAsync(request.PostcodeList.Postcodes);

            if (!request.IncludeAddressDetails)
            {
                foreach (PostcodeDto postcodeDto in postcodesDto)
                {
                    postcodeDto.AddressDetails = null;
                }
            }

            foreach (PostcodeDto postcodeDto in postcodesDto)
            {
                getPostcodesResponse.PostcodesResponse.Add(postcodeDto.Postcode, _mapper.Map<PostcodeDto, GetPostcodeResponse>(postcodeDto));
            }

            return getPostcodesResponse;
        }
    }
}
