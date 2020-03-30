using AddressService.Core.Domains.Entities.Response;
using AddressService.Core.Dto;

namespace AddressService.Mappers
{
    public interface IQasMapper
    {
        PostcodeResponse MapResponse(QasRootResponse qasRootResponse);
    }
}