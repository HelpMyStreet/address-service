using AddressService.Core.Dto;
using System.Collections.Generic;
using System.Linq;

namespace AddressService.Mappers
{
    public interface IQasMapper
    {
        PostcodeDto MapToPostcodeDto(string postcode, IEnumerable<QasFormatRootResponse> qasFormatRootResponses);

        ILookup<string, string> GetFormatIds(IEnumerable<QasSearchRootResponse> qasSearchRootResponses);
    }
}