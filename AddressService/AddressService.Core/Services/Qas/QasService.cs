using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AddressService.Core.Config;
using AddressService.Core.Dto;
using AddressService.Core.Utils;
using Marvin.StreamExtensions;

namespace AddressService.Core.Services.Qas
{
    public class QasService : IQasService
    {
        private readonly IHttpClientWrapper _httpClientWrapper;

        public QasService(IHttpClientWrapper httpClientWrapper)
        {
            _httpClientWrapper = httpClientWrapper;
        }

        public async Task<QasSearchRootResponse> GetGlobalIntuitiveSearchResponseAsync(string postcode, CancellationToken cancellationToken)
        {
            postcode = postcode.Replace(" ", "");

            string path = $"capture/address/v2/search";
            string query = $"query={postcode}&country=GBR&take=100"; 
            string absolutePath = $"{path}?{query}";

            QasSearchRootResponse qasSearchRootResponse;
            using (HttpResponseMessage response = await _httpClientWrapper.GetAsync(HttpClientConfigName.Qas, absolutePath, cancellationToken).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                Stream stream = await response.Content.ReadAsStreamAsync();
                qasSearchRootResponse = stream.ReadAndDeserializeFromJson<QasSearchRootResponse>();
            }
            qasSearchRootResponse.Postcode = PostcodeFormatter.FormatPostcode(postcode);

            return qasSearchRootResponse;
        }

        public async Task<QasFormatRootResponse> GetGlobalIntuitiveFormatResponseAsync(string id, CancellationToken cancellationToken)
        {

            string path = $"capture/address/v2/format";
            string query = $"country=GBR&id={id}";
            string absolutePath = $"{path}?{query}";

            QasFormatRootResponse qasSearchRootResponse;
            using (HttpResponseMessage response = await _httpClientWrapper.GetAsync(HttpClientConfigName.Qas, absolutePath, cancellationToken).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                Stream stream = await response.Content.ReadAsStreamAsync();
                qasSearchRootResponse = stream.ReadAndDeserializeFromJson<QasFormatRootResponse>();
            }

            return qasSearchRootResponse;
        }

     
    }
}
