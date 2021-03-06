﻿using AddressService.Core.Dto;
using AddressService.Core.Interfaces.Repositories;
using AddressService.Handlers.BusinessLogic;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AddressService.Handlers
{
    public class GetNumberOfAddressesPerPostcodeInBoundaryHandler : IRequestHandler<GetNumberOfAddressesPerPostcodeInBoundaryRequest, GetNumberOfAddressesPerPostcodeInBoundaryResponse>
    {
        private readonly IRepository _repository;
        private readonly IPostcodeAndAddressGetter _postcodeAndAddressGetter;

        public GetNumberOfAddressesPerPostcodeInBoundaryHandler(IRepository repository, IPostcodeAndAddressGetter postcodeAndAddressGetter)
        {
            _repository = repository;
            _postcodeAndAddressGetter = postcodeAndAddressGetter;
        }

        public async Task<GetNumberOfAddressesPerPostcodeInBoundaryResponse> Handle(GetNumberOfAddressesPerPostcodeInBoundaryRequest request, CancellationToken cancellationToken)
        {
            IEnumerable<string> postCodesWithinBoundary = await _repository.GetPostcodesInBoundaryAsync(request.SwLatitude, request.SwLongitude, request.NeLatitude, request.NeLongitude);
            
            IEnumerable<PostcodeWithNumberOfAddressesDto> postCodesWithNumberOfAddresses = await _postcodeAndAddressGetter.GetNumberOfAddressesPerPostcodeAsync(postCodesWithinBoundary, cancellationToken);

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
