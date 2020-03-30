using AddressService.Core.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AddressService.Core.Interfaces.Repositories
{
    public interface IRepository
    {
        Task<IEnumerable<PostcodeDto>> GetPostcodes(IEnumerable<string> postCodes);
        Task SavePostcodes(IEnumerable<PostcodeDto> postCodes);
    }
}
