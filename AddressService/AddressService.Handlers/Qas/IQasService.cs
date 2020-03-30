using System.Threading.Tasks;
using AddressService.Core.Dto;

namespace AddressService.Handlers.Qas
{
    public interface IQasService
    {
        Task<QasRootResponse> GetGlobalIntuitiveSearchResponse(string postcode);
    }
}