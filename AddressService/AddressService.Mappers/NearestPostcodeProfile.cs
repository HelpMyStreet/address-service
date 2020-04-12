using AddressService.Core.Contracts;
using AddressService.Core.Dto;
using AutoMapper;

namespace AddressService.Mappers
{
    public class NearestPostcodeProfile : Profile
    {
        public NearestPostcodeProfile()
        {
            CreateMap<NearestPostcodeDto, NearestPostcodeWithoutAddress>();
              //  .ForMember(s => s.DistanceInMiles, c => c.Ignore());
        }
    }
}
