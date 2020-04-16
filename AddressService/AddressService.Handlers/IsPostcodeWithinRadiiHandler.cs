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
using AddressService.Core.Interfaces.Repositories;

namespace AddressService.Handlers
{
    public class IsPostcodeWithinRadiiHandler : IRequestHandler<IsPostcodeWithinRadiiRequest, IsPostcodeWithinRadiiResponse>
    {
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        private readonly IRepository _repository;

        private static Dictionary<string, LatLongDto> _postcodesWithLatitudeAndLongitudes;

        public IsPostcodeWithinRadiiHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<IsPostcodeWithinRadiiResponse> Handle(IsPostcodeWithinRadiiRequest request, CancellationToken cancellationToken)
        {
            if (_postcodesWithLatitudeAndLongitudes == null)
            {
                try
                {
                    await _lock.WaitAsync(cancellationToken);
                    if (_postcodesWithLatitudeAndLongitudes == null)
                    {
                        IEnumerable<PostcodeWithLatLongDto> postcodesWithLatitudeAndLongitudes = await _repository.GetAllPostcodeLatitudesAndLongitudesAsync();
                        _postcodesWithLatitudeAndLongitudes = postcodesWithLatitudeAndLongitudes.ToDictionary(x => x.Postcode, x => new LatLongDto(x.Latitude, x.Longitude));
                    }
                }
                finally
                {
                    _lock.Release();
                }
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();
            LatLongDto postcodeToCompareToLatLong = _postcodesWithLatitudeAndLongitudes[request.Postcode];


            List<int> idsInRadius = new List<int>(request.PostcodeWithRadiuses.Count / 100);

            foreach (var p in request.PostcodeWithRadiuses)
            {
                if (_postcodesWithLatitudeAndLongitudes.TryGetValue(p.Postcode, out var postcodeWithLatLong))
                {
                    var isWithinRadius = DistanceCalculator.GetDistance(postcodeToCompareToLatLong.Latitude, postcodeToCompareToLatLong.Longitude, postcodeWithLatLong.Latitude, postcodeWithLatLong.Longitude) <= p.RadiusInMetres;

                    if (isWithinRadius)
                    {
                        idsInRadius.Add(p.Id);
                    }
                }
            }

            //List<int> result = (from pc in _postcodesWithLatitudeAndLongitudes
            //              join pcr in request.PostcodeWithRadiuses
            //                  on pc.Key equals pcr.Postcode
            //              where DistanceCalculator.GetDistance(postcodeToCompareToLatLong.Latitude, postcodeToCompareToLatLong.Longitude, pc.Value.Latitude, pc.Value.Longitude) <= pcr.RadiusInMetres
            //              select pcr.Id).ToList();


            IsPostcodeWithinRadiiResponse isPostcodeWithinRadiiResponse = new IsPostcodeWithinRadiiResponse()
            {
                IdsWithinRadius = idsInRadius
            };

            sw.Stop();
            Debug.WriteLine($"join: {sw.ElapsedMilliseconds}");

            return isPostcodeWithinRadiiResponse;
        }


    }

}
