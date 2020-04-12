using AddressService.Core.Config;
using AddressService.Core.Dto;
using AddressService.Core.Services.PostcodeIo;
using AutoMapper;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Utils.Utils;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AddressService.Core.Utils;

namespace AddressService.Handlers
{
    public class GetNearbyPostcodesHandler : IRequestHandler<GetNearbyPostcodesRequest, GetNearbyPostcodesResponse>
    {
        private readonly IPostcodeIoService _postcodeIoService;
        private readonly IMapper _mapper;
        private readonly IPostcodeGetter _postcodeGetter;
        private readonly IAddressDetailsSorter _addressDetailsSorter;
        private readonly IOptionsSnapshot<ApplicationConfig> _applicationConfig;

        public GetNearbyPostcodesHandler(IPostcodeIoService postcodeIoService, IMapper mapper, IPostcodeGetter postcodeGetter, IAddressDetailsSorter addressDetailsSorter, IOptionsSnapshot<ApplicationConfig> applicationConfig)
        {
            _postcodeIoService = postcodeIoService;
            _mapper = mapper;
            _postcodeGetter = postcodeGetter;
            _addressDetailsSorter = addressDetailsSorter;
            _applicationConfig = applicationConfig;
        }

        public async Task<GetNearbyPostcodesResponse> Handle(GetNearbyPostcodesRequest request, CancellationToken cancellationToken)
        {
            string postcode = PostcodeFormatter.FormatPostcode(request.Postcode);

            // call Postcode IO for nearest postcodes
            PostCodeIoNearestRootResponse postCodeIoResponse = await _postcodeIoService.GetNearbyPostCodesAsync(postcode, cancellationToken);

            postCodeIoResponse.Result.ForEach(x => x.Postcode = PostcodeFormatter.FormatPostcode(x.Postcode));

            IEnumerable<string> nearestPostcodes = postCodeIoResponse.Result.OrderBy(x => x.Distance)
                .Take(_applicationConfig.Value.DefaultMaxNumberOfNearbyPostcodes)
                .Select(x => x.Postcode).ToList();

            // get postcodes
            IEnumerable<PostcodeDto> postcodeDtos = await _postcodeGetter.GetPostcodesAsync(nearestPostcodes, cancellationToken);

            // create response
            GetNearbyPostcodesResponse getNearbyPostcodesResponse = new GetNearbyPostcodesResponse();

            IEnumerable<GetNearbyPostCodeResponse> getNearbyPostCodeResponses = _mapper.Map<IEnumerable<PostcodeDto>, IEnumerable<GetNearbyPostCodeResponse>>(postcodeDtos);

            getNearbyPostcodesResponse.Postcodes =
                (from getNearbyPostCodeResponse in getNearbyPostCodeResponses
                 join postCodeIoResult in postCodeIoResponse.Result
                     on getNearbyPostCodeResponse.Postcode equals postCodeIoResult.Postcode
                 select new GetNearbyPostCodeResponse
                 {
                     Postcode = getNearbyPostCodeResponse.Postcode,
                     FriendlyName = getNearbyPostCodeResponse.FriendlyName,
                     AddressDetails = _addressDetailsSorter.OrderAddressDetailsResponse(getNearbyPostCodeResponse.AddressDetails),
                     DistanceInMetres = (int)Math.Round(postCodeIoResult.Distance, 0)
                 })
                .OrderBy(x => x.DistanceInMetres)
                .ToList();


            return getNearbyPostcodesResponse;
        }

        // method that gets postcodes from DB - this will return retired postcodes
        //public async Task<GetNearbyPostcodesResponse> Handle(GetNearbyPostcodesRequest request, CancellationToken cancellationToken)
        //{
        //    string postcode = PostcodeFormatter.FormatPostcode(request.Postcode);

        //    // get nearest postcodes
        //    IReadOnlyList<NearestPostcodeDto> nearestPostcodeDtos = await _nearestPostcodeGetter.GetNearestPostcodesAsync(postcode);

        //    IEnumerable<string> nearestPostcodes = nearestPostcodeDtos.Select(x => x.Postcode).ToList();

        //    // get postcodes
        //    IEnumerable<PostcodeDto> postcodeDtos = await _postcodeGetter.GetPostcodesAsync(nearestPostcodes, cancellationToken);

        //    // create response
        //    GetNearbyPostcodesResponse getNearbyPostcodesResponse = new GetNearbyPostcodesResponse();

        //    IEnumerable<GetNearbyPostCodeResponse> getNearbyPostCodeResponses = _mapper.Map<IEnumerable<PostcodeDto>, IEnumerable<GetNearbyPostCodeResponse>>(postcodeDtos);

        //    getNearbyPostcodesResponse.Postcodes =
        //        (from getNearbyPostCodeResponse in getNearbyPostCodeResponses
        //            join nearestPostcodeDto in nearestPostcodeDtos
        //                on getNearbyPostCodeResponse.Postcode equals nearestPostcodeDto.Postcode
        //            select new GetNearbyPostCodeResponse
        //            {
        //                Postcode = getNearbyPostCodeResponse.Postcode,
        //                AddressDetails = _addressDetailsSorter.OrderAddressDetailsResponse(getNearbyPostCodeResponse.AddressDetails),
        //                DistanceInMetres = nearestPostcodeDto.DistanceInMetres,
        //                // DistanceInMiles = nearestPostcodeDto.DistanceInMetres,
        //            })
        //        .OrderBy(x => x.DistanceInMetres)
        //        .ToList();


        //    return getNearbyPostcodesResponse;
        //}
    }
}
