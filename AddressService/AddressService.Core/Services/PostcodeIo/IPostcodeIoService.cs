using System.Threading;
using System.Threading.Tasks;
using AddressService.Core.Dto;

namespace AddressService.Core.Services.PostcodeIo
{
    public interface IPostcodeIoService
    {
        Task<PostCodeIoNearestRootResponse> GetNearbyPostCodesAsync(string postcode, CancellationToken cancellationToken);

        Task<bool> IsPostcodeValidAsync(string postcode, CancellationToken cancellationToken);
    }
}