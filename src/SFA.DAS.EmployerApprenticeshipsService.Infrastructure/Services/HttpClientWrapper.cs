using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using SFA.DAS.EAS.Domain.Http;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        public string AuthScheme { get; set; }
        public string BaseUrl { get; set; }
        public List<MediaTypeWithQualityHeaderValue> MediaTypeWithQualityHeaderValueList { get; set; }
        private readonly ILogger _logger;

        public HttpClientWrapper(ILogger logger)
        {
            _logger = logger;
            MediaTypeWithQualityHeaderValueList = new List<MediaTypeWithQualityHeaderValue>();
        }

        public async Task<string> SendMessage<T>(T content, string url)
        {
            using (var httpClient = CreateHttpClient())
            {
                var serializeObject = JsonConvert.SerializeObject(content);
                var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(serializeObject, Encoding.UTF8, "application/json")
                });
                EnsureSuccessfulResponse(response);

                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<T> Get<T>(string authToken, string url)
        {
            using (var httpClient = CreateHttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthScheme, authToken);

                var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));
                EnsureSuccessfulResponse(response);

                return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
            }
        }

        public async Task<string> GetString(string url)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                EnsureSuccessfulResponse(response);

                return response.Content.ReadAsStringAsync().Result;
            }
        }

        private HttpClient CreateHttpClient()
        {
            if (string.IsNullOrEmpty(BaseUrl))
            {
                throw new ArgumentNullException(nameof(BaseUrl));
            }

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)

            };

            if (MediaTypeWithQualityHeaderValueList.Any())
            {
                foreach (var mediaTypeWithQualityHeaderValue in MediaTypeWithQualityHeaderValueList)
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(mediaTypeWithQualityHeaderValue);
                }
            }

            return httpClient;
        }

        private void EnsureSuccessfulResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new ResourceNotFoundException(response.RequestMessage.RequestUri.ToString());
                case 429:
                    throw new TooManyRequestsException();
                case 500:
                    throw new ServiceUnavailableException();
                default:
                    throw new HttpException((int)response.StatusCode, $"Unexpected HTTP exception - ({(int)response.StatusCode}): {response.ReasonPhrase}");
            }
        }
    }
}