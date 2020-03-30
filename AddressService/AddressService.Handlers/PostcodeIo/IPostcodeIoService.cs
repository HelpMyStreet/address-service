using AddressService.Core.Dto;
using System.Threading.Tasks;

namespace AddressService.Handlers.PostcodeIo
{
    public interface IPostcodeIoService
    {
        Task<PostCodeIoNearestRootResponse> GetNearbyPostCodesAsync(string postcode);
    }
}