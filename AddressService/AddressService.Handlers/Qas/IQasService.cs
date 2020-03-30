using System.Threading.Tasks;
using AddressService.Core.Dto;

namespace AddressService.Handlers.Qas
{
    public interface IQasService
    {
        Task<QasSearchRootResponse> GetGlobalIntuitiveSearchResponse(string postcode);

        Task<QasFormatRootResponse> GetGlobalIntuitiveFormatResponse(string id);
    }
}