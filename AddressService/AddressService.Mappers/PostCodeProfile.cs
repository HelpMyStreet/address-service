using AddressService.Core.Domains.Entities.Response;
using AddressService.Core.Dto;
using AddressService.Repo.EntityFramework.Entities.AddressService.Repo.EntityFramework.Entities;
using AutoMapper;

namespace AddressService.Mappers
{
    public class PostCodeProfile : Profile
    {
        public PostCodeProfile()
        {
            CreateMap<PostcodeEntity, PostcodeDto>()
                .ForMember(dest => dest.AddressDetails, opt => opt.Ignore());
            CreateMap<PostcodeDto, PostcodeEntity>()
                .ForMember(dest => dest.AddressDetails, opt => opt.Ignore());
            CreateMap<PostcodeDto, PostcodeResponse>()
                .ForMember(dest => dest.AddressDetails, opt => opt.Ignore());
        }
    }
}
