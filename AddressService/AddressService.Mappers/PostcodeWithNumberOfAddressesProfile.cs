using AddressService.Core.Contracts;
using AddressService.Core.Dto;
using AutoMapper;

namespace AddressService.Mappers
{
    public class PostcodeWithNumberOfAddressesProfile : Profile
    {
        public PostcodeWithNumberOfAddressesProfile()
        {
            CreateMap<PostcodeWithNumberOfAddressesDto, PostcodeWithNumberOfAddresses>();
        }
    }
}
