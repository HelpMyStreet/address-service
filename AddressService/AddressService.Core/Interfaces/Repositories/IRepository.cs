using AddressService.Core.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AddressService.Core.Interfaces.Repositories
{
    public interface IRepository
    {
        Task<IEnumerable<PostcodeDto>> GetPostcodesAsync(IEnumerable<string> postCodes);
        Task SavePostcodesAsync(IEnumerable<PostcodeDto> postCodes);

        Task<bool> IsPostcodeInDb(string postcode);
    }
}
