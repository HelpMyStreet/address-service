using AddressService.Core.Dto;
using AddressService.Repo.EntityFramework.Entities;
using AutoMapper;
using HelpMyStreet.Contracts.AddressService.Response;

namespace AddressService.Mappers
{
    public class PostCodeProfile : Profile
    {
        public PostCodeProfile()
        {
            CreateMap<PostcodeEntity, PostcodeDto>()
                .ForMember(s => s.AddressDetails, c => c.MapFrom(m => m.AddressDetails))
                .ForMember(s => s.Coordinates, c => c.Ignore());
            

            CreateMap<PostcodeDto, PostcodeEntity>()
                .ForMember(s => s.AddressDetails, c => c.MapFrom(m => m.AddressDetails))
                .ForMember(s => s.Coordinates, c => c.Ignore());

            CreateMap<PostcodeDto, GetPostcodeResponse>()
                .ForMember(s => s.AddressDetails, c => c.MapFrom(m => m.AddressDetails));

            CreateMap<PostcodeDto, GetNearbyPostCodeResponse>()
              .ForMember(s => s.AddressDetails, c => c.MapFrom(m => m.AddressDetails))
            .ForMember(s => s.DistanceInMetres, c => c.Ignore());
        }
    }
}
