using AddressService.Core.Dto;
using AddressService.Repo.EntityFramework.Entities;
using AutoMapper;
namespace AddressService.Mappers
{
    public class PreComputedNearestPostcodeProfile : Profile
    {
        public PreComputedNearestPostcodeProfile()
        {
            CreateMap<PreComputedNearestPostcodesDto, PreComputedNearestPostcodesEntity>();
            CreateMap<PreComputedNearestPostcodesEntity, PreComputedNearestPostcodesDto>();
        }
    }
}
