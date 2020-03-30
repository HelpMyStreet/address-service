using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AddressService.Core.Config;
using AddressService.Core.Dto;
using AddressService.Core.Utils;
using Marvin.StreamExtensions;

namespace AddressService.Handlers.Qas
{
    public class QasService : IQasService
    {
        private readonly IHttpClientWrapper _httpClientWrapper;

        public QasService(IHttpClientWrapper httpClientWrapper)
        {
            _httpClientWrapper = httpClientWrapper;
        }

        public async Task<QasRootResponse> GetGlobalIntuitiveSearchResponse(string postcode)
        {
            postcode = postcode.Replace(" ", "");

            string path = $"capture/address/v2/search";
            string query = $"query={postcode}&country=GBR&take=100"; 
            string absolutePath = $"{path}?{query}";

            QasRootResponse qasRootResponse;
            using (HttpResponseMessage response = await _httpClientWrapper.GetAsync(HttpClientConfigName.Qas, absolutePath).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                Stream stream = await response.Content.ReadAsStreamAsync();
                qasRootResponse = stream.ReadAndDeserializeFromJson<QasRootResponse>();
            }
            qasRootResponse.Postcode = postcode;

            return qasRootResponse;
        }
    }
}
