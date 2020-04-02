﻿using AddressService.Core.Domains.Entities.Response;
using AddressService.Core.Dto;
using AddressService.Repo.EntityFramework.Entities;
using AutoMapper;

namespace AddressService.Mappers
{
    public class AddressDetailsProfile : Profile
    {
        public AddressDetailsProfile()
        {
            CreateMap<AddressDetailsEntity, AddressDetailsDto>();
            CreateMap<AddressDetailsDto, AddressDetailsEntity>();
            CreateMap<AddressDetailsDto, AddressDetailsResponse>()
                .ForMember(s => s.Postcode, c => c.MapFrom(m => m.PostCode.Postcode));
        }
    }
}
