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
using AddressService.Core.Interfaces.Repositories;
using System.Collections.Generic;
using System;
using System.Linq;
using UserService.Core.Utils;

namespace AddressService.Handlers
{
    public class GetDistanceBetweenPostcodesHandler : IRequestHandler<GetDistanceBetweenPostcodesRequest, GetDistanceBetweenPostcodesResponse>
    {
        private readonly IRepository _repository;
        private readonly IDistanceCalculator _distanceCalculator;
        
        public GetDistanceBetweenPostcodesHandler(IRepository repository, IDistanceCalculator distanceCalculator)
        {
            _repository = repository;
            _distanceCalculator = distanceCalculator;
        }

        public async Task<GetDistanceBetweenPostcodesResponse> Handle(GetDistanceBetweenPostcodesRequest request, CancellationToken cancellationToken)
        {
            request.Postcode1 = PostcodeFormatter.FormatPostcode(request.Postcode1);
            request.Postcode2 = PostcodeFormatter.FormatPostcode(request.Postcode2);

            var postcodeDetails = await _repository.GetPostcodeCoordinatesAsync(new List<string>() { request.Postcode1, request.Postcode2 });

            if (postcodeDetails == null)
            {
                throw new Exception($"Unable to retrieve post code details for {request.Postcode1} or {request.Postcode2}");
            }

            if (postcodeDetails.Count() != 2)
            {
                throw new Exception($"Only expecting 2 row in collection for {request.Postcode1} and {request.Postcode2}. {postcodeDetails.Count()} rows returned");
            }

            var postcodeDetails1 = postcodeDetails.FirstOrDefault(x => x.Postcode == request.Postcode1);
            var postcodeDetails2 = postcodeDetails.FirstOrDefault(x => x.Postcode == request.Postcode2);

            if(postcodeDetails1 == null || postcodeDetails2 == null)
            {
                throw new Exception($"Either postcode details not returned for  {request.Postcode1} and {request.Postcode2}.");
            }

            var distanceInMiles = _distanceCalculator.GetDistanceInMiles(
                postcodeDetails1.Latitude,
                postcodeDetails1.Longitude,
                postcodeDetails2.Latitude,
                postcodeDetails2.Longitude
                );

            return new GetDistanceBetweenPostcodesResponse()
            {
                DistanceInMiles = distanceInMiles
            };
        }
    }
}
