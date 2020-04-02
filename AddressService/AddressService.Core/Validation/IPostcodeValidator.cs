using System.Threading.Tasks;

namespace AddressService.Core.Validation
{
    public interface IPostcodeValidator
    {
        Task<bool> IsPostcodeValidAsync(string postcode);
    }
}