using AutoMapper;
using AddressService.Core.Dto;
using AddressService.Repo.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AddressService.Mappers
{
    public class PostCodeProfile : Profile
    {
        public PostCodeProfile()
        {
            CreateMap<PostCode, PostcodeDTO>()
                .ForMember(dest => dest.AddressDetails, opt => opt.Ignore());
            CreateMap<PostcodeDTO, PostCode>()
                .ForMember(dest => dest.AddressDetails, opt => opt.Ignore());
        }
    }
}
