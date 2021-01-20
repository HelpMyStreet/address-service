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
    public class GetLocationsByDistanceHandler : IRequestHandler<GetLocationsByDistanceRequest, GetLocationsByDistanceResponse>
    {
        private readonly IRepository _repository;
        
        public GetLocationsByDistanceHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetLocationsByDistanceResponse> Handle(GetLocationsByDistanceRequest request, CancellationToken cancellationToken)
        {
            var postcodeDetails = await _repository.GetPostcodeCoordinatesAsync(new List<string>() { request.Postcode });

            if(postcodeDetails == null)
            {
                throw new Exception($"Unable to retrieve post code details for {request.Postcode}");
            }

            if(postcodeDetails.Count()!=1)
            {
                throw new Exception($"Only expecting 1 row in collection for {request.Postcode}. {postcodeDetails.Count()} rows returned");
            }

            var allLocations = _repository.GetAllLocations();

            if (allLocations == null && allLocations.Count()==0)
            {
                throw new Exception($"Unable to retrieve any locations");
            }

            DistanceCalculator distanceCalculator = new DistanceCalculator();
            PostcodeWithCoordinatesDto requestPostCode = postcodeDetails.First();

            var locationDistances =  allLocations.Where(x => distanceCalculator.GetDistanceInMiles(
                (double)requestPostCode.Latitude,
                (double)requestPostCode.Longitude,
                (double)x.Latitude,
                (double)x.Longitude) <= request.MaxDistance)
                .Select(x=> new LocationDistance()
                {
                    Location = x.Location,
                    DistanceFromPostCode = distanceCalculator.GetDistanceInMiles(
                        requestPostCode.Latitude,
                        requestPostCode.Longitude,
                        (double)x.Latitude,
                        (double)x.Longitude)
                }).ToList();

            return new GetLocationsByDistanceResponse()
            {
                LocationDistances = locationDistances
            };
        }
    }
}
