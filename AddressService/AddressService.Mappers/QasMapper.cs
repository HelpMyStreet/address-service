using AddressService.Core.Domains.Entities.Response;
using AddressService.Core.Dto;
using System.Linq;

namespace AddressService.Mappers
{
    public class QasMapper : IQasMapper
    {
        public PostCodeResponse MapResponse(QasRootResponse qasRootResponse)
        {
            var postcodeResponse = new PostCodeResponse()
            {
                PostCode = qasRootResponse.Postcode,
                Addresses = qasRootResponse.Results.Select(y => y.Suggestion).ToList()
            };

            return postcodeResponse;
        }
    }
}
