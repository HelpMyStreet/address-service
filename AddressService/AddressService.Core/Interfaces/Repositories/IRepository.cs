﻿using AddressService.Core.Dto;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AddressService.Core.Interfaces.Repositories
{
    public interface IRepository
    {
        List<LocationDetails> GetAllLocations();
        Task<LocationDetails> GetLocationDetails(Location location);
        Task<IEnumerable<PostcodeDto>> GetPostcodesAsync(IEnumerable<string> postcodes);
        Task SaveAddressesAndFriendlyNameAsync(IEnumerable<PostcodeDto> postCodes);
        Task<bool> IsPostcodeInDbAndActive(string postcode);
        Task<IEnumerable<NearestPostcodeDto>> GetNearestPostcodesAsync(string postcode, double distanceInMetres);
        Task SavePreComputedNearestPostcodes(PreComputedNearestPostcodesDto preComputedNearestPostcodesDto);
        Task<PreComputedNearestPostcodesDto> GetPreComputedNearestPostcodes(string postcode);
        Task<IEnumerable<PostcodeWithCoordinatesDto>> GetPostcodeCoordinatesAsync(IEnumerable<string> postcodes);
        Task<IEnumerable<PostcodeWithNumberOfAddressesDto>> GetNumberOfAddressesPerPostcodeAsync(IEnumerable<string> postcodes);
        Task<IEnumerable<string>> GetPostcodesInBoundaryAsync(double swLatitude, double swLongitude, double neLatitude, double neLongitude);
    }
}
