using AddressService.Core.Dto;
using AddressService.Repo.EntityFramework.Entities;
using AutoMapper;
using HelpMyStreet.Contracts.AddressService.Response;

namespace AddressService.Mappers
{
    public class AddressDetailsProfile : Profile
    {
        public AddressDetailsProfile()
        {
            CreateMap<AddressDetailsEntity, AddressDetailsDto>()
                .ForMember(x=>x.Postcode,x=> x.MapFrom(m => m.PostCode.Postcode));
            CreateMap<AddressDetailsDto, AddressDetailsEntity>()
                .ForMember(x=>x.PostCode,y=>y.Ignore());
            CreateMap<AddressDetailsDto, AddressDetailsResponse>()
                .ForMember(s => s.Postcode, c => c.MapFrom(m => m.Postcode));
        }
    }
}
