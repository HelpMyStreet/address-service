using System.Threading.Tasks;
using AddressService.Core.Dto;

namespace AddressService.Handlers.Qas
{
    public interface IQasService
    {
        Task<QasSearchRootResponse> GetGlobalIntuitiveSearchResponseAsync(string postcode);

        Task<QasFormatRootResponse> GetGlobalIntuitiveFormatResponseAsync(string id);
    }
}