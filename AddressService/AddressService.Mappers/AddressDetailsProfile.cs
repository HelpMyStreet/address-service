using AutoMapper;
using AddressService.Core.Dto;
using AddressService.Repo.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AddressService.Mappers
{
    public class AddressDetailsProfile : Profile
    {
        public AddressDetailsProfile()
        {
            CreateMap<AddressDetails, AddressDetailsDTO>();
            CreateMap<AddressDetailsDTO, AddressDetails>();
        }
    }
}
