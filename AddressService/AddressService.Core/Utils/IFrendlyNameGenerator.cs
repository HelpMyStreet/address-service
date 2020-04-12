using AddressService.Core.Dto;

namespace AddressService.Core.Utils
{
    public interface IFriendlyNameGenerator
    {
        void GenerateFriendlyName(PostcodeDto postcodeDto);
    }
}
