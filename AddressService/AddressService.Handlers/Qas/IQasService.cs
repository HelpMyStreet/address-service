using System.Threading;
using System.Threading.Tasks;
using AddressService.Core.Dto;

namespace AddressService.Handlers.Qas
{
    public interface IQasService
    {
        Task<QasSearchRootResponse> GetGlobalIntuitiveSearchResponseAsync(string postcode, CancellationToken cancellationToken);

        Task<QasFormatRootResponse> GetGlobalIntuitiveFormatResponseAsync(string id, CancellationToken cancellationToken);
    }
}