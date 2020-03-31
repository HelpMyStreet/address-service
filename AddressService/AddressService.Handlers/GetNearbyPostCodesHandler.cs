﻿using AddressService.Core.Domains.Entities.Request;
using AddressService.Core.Domains.Entities.Response;
using MediatR;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AddressService.Core.Dto;
using AddressService.Core.Interfaces.Repositories;
using AddressService.Core.Utils;
using AddressService.Handlers.PostcodeIo;
using AddressService.Handlers.Qas;
using AddressService.Mappers;
using AutoMapper;

namespace AddressService.Handlers
{
    public class GetNearbyPostcodesHandler : IRequestHandler<GetNearbyPostcodesRequest, GetNearbyPostcodesResponse>
    {
        private readonly IPostcodeIoService _postcodeIoService;
        private readonly IMapper _mapper;
        private readonly IPostcodeGetter _postcodeGetter;

        public GetNearbyPostcodesHandler(IPostcodeIoService postcodeIoService, IMapper mapper, IPostcodeGetter postcodeGetter)
        {
            _postcodeIoService = postcodeIoService;
            _mapper = mapper;
            _postcodeGetter = postcodeGetter;
        }

        public async Task<GetNearbyPostcodesResponse> Handle(GetNearbyPostcodesRequest request, CancellationToken cancellationToken)
        {
            // call Postcode IO for nearest postcodes
            string postcode = PostcodeCleaner.CleanPostcode(request.Postcode);

            PostCodeIoNearestRootResponse postCodeIoResponse = await _postcodeIoService.GetNearbyPostCodesAsync(postcode, cancellationToken);

            ImmutableHashSet<string> nearestPostcodes = postCodeIoResponse.Result.OrderBy(x => x.Distance).Select(x => PostcodeCleaner.CleanPostcode(x.Postcode)).ToImmutableHashSet();

            // get postcodes
            IEnumerable<PostcodeDto> postcodeDtos = await _postcodeGetter.GetPostcodesAsync(nearestPostcodes, cancellationToken);

            // create response
            GetNearbyPostcodesResponse getNearbyPostcodesResponse = new GetNearbyPostcodesResponse();
            getNearbyPostcodesResponse.Postcodes = _mapper.Map<IEnumerable<PostcodeDto>, IEnumerable<PostcodeResponse>>(postcodeDtos);
            
            return getNearbyPostcodesResponse;
        }
    }
}