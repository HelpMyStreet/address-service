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
            CreateMap<PostcodeEntity, PostcodeDTO>()
                .ForMember(dest => dest.AddressDetails, opt => opt.Ignore());
            CreateMap<PostcodeDTO, PostcodeEntity>()
                .ForMember(dest => dest.AddressDetails, opt => opt.Ignore());
            CreateMap<PostcodeDTO, PostcodeResponse>()
                .ForMember(dest => dest.AddressDetails, opt => opt.Ignore());
        }
    }
}
