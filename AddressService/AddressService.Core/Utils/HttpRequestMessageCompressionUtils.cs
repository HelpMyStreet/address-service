using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AddressService.Core.Utils
{
    public class HttpRequestMessageCompressionUtils
    {
        /// <summary>
        /// Decompresses HttpRequestMessage if compressed with gzip and deserialises content.  Uses Utf8Json serialiser for speed.
        /// </summary>
        /// <typeparam name="T">Type to deserialise to</typeparam>
        /// <param name="httpRequestMessage"></param>
        /// <returns></returns>
        public static async Task<T> DeserialiseAsync<T>(HttpRequestMessage httpRequestMessage)
        {
            T deserialisedContent;

            if (httpRequestMessage.Content.Headers.ContentEncoding.Any(x => x.ToLower() == "gzip"))
            {
                Stream inputStream = await httpRequestMessage.Content.ReadAsStreamAsync();
                using (GZipStream decompressionStream = new GZipStream(inputStream, CompressionMode.Decompress))
                {
                    deserialisedContent = await Utf8Json.JsonSerializer.DeserializeAsync<T>(decompressionStream);
                }
            }
            else
            {
                Stream stream = await httpRequestMessage.Content.ReadAsStreamAsync();
                deserialisedContent = await Utf8Json.JsonSerializer.DeserializeAsync<T>(stream);
            }

            return deserialisedContent;
        }
    }
}
