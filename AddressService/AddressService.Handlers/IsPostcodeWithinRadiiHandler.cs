using System;
using System.Collections.Concurrent;
using AddressService.Core.Dto;
using AddressService.Core.Utils;
using AutoMapper;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Utils.Utils;
using MediatR;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AddressService.Core.Contracts;
using AddressService.Core.Extensions;
using AddressService.Core.Interfaces.Repositories;

namespace AddressService.Handlers
{
    public class IsPostcodeWithinRadiiHandler : IRequestHandler<IsPostcodeWithinRadiiRequest, IsPostcodeWithinRadiiResponse>
    {

        private readonly IPostcodeCoordinatesGetter _postcodeCoordinatesGetter;

        public IsPostcodeWithinRadiiHandler(IPostcodeCoordinatesGetter postcodeCoordinatesGetter)
        {
            _postcodeCoordinatesGetter = postcodeCoordinatesGetter;
        }

        public async Task<IsPostcodeWithinRadiiResponse> Handle(IsPostcodeWithinRadiiRequest request, CancellationToken cancellationToken)
        {
            var sw = new Stopwatch();
            sw.Start();

            request.Postcode = PostcodeFormatter.FormatPostcode(request.Postcode);

            var requiredPostcodes = request.PostcodeWithRadiuses.Select(x => x.Postcode).ToHashSet();
            requiredPostcodes.Add(request.Postcode);

            var postcodeCoordinates = await _postcodeCoordinatesGetter.GetPostcodeCoordinates(requiredPostcodes);

            // this shouldn't return null due to the postcode not being in the dictionary as it will have been validated at the beginning of the request
            postcodeCoordinates.TryGetValue(request.Postcode, out CoordinatesDto postcodeToCompareToLatitudeLongitude);

            List<int> idsInRadius = new List<int>(request.PostcodeWithRadiuses.Count / 100);

            foreach (PostcodeWithRadius p in request.PostcodeWithRadiuses)
            {
                if (postcodeCoordinates.TryGetValue(p.Postcode, out CoordinatesDto postcodeWithLatLong))
                {
                    bool isWithinRadius = DistanceCalculator.GetDistance(postcodeToCompareToLatitudeLongitude.Latitude, postcodeToCompareToLatitudeLongitude.Longitude, postcodeWithLatLong.Latitude, postcodeWithLatLong.Longitude) <= p.RadiusInMetres;

                    if (isWithinRadius)
                    {
                        idsInRadius.Add(p.Id);
                    }
                }
            }

            IsPostcodeWithinRadiiResponse isPostcodeWithinRadiiResponse = new IsPostcodeWithinRadiiResponse()
            {
                IdsWithinRadius = idsInRadius
            };

            sw.Stop();
            Debug.WriteLine($"IsPostcodeWithinRadiiHandler Handle: {sw.ElapsedMilliseconds}");

            return isPostcodeWithinRadiiResponse;
        }


    }

}
