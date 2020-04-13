using AddressService.Core.Dto;
using AutoMapper;
using HelpMyStreet.Contracts.AddressService.Response;

namespace AddressService.Mappers
{
    public class NearestPostcodeProfile : Profile
    {
        public NearestPostcodeProfile()
        {
            CreateMap<NearestPostcodeDto, NearestPostcodeWithoutAddress>();
        }
    }
}
