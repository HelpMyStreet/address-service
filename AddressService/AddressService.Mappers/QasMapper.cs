using AddressService.Core.Domains.Entities.Response;
using AddressService.Core.Dto;
using System.Linq;

namespace AddressService.Mappers
{
    public class QasMapper : IQasMapper
    {
        public PostcodeResponse MapResponse(QasRootResponse qasRootResponse)
        {
            var postcodeResponse = new PostcodeResponse()
            {
                Postcode = qasRootResponse.Postcode,
                Addresses = qasRootResponse.Results.Select(y => y.Suggestion).ToList()
            };

            return postcodeResponse;
        }
    }
}
