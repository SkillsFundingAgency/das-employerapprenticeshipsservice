using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Services
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly ILogger _logger;
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;

        public HttpClientWrapper(ILogger logger, EmployerApprenticeshipsServiceConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<string> SendMessage<T>(T content, string url)
        {
            try
            {
                using (var httpClient = CreateHttpClient())
                {

                    var serializeObject = JsonConvert.SerializeObject(content);
                    var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, url)
                    {
                        Content = new StringContent(serializeObject, Encoding.UTF8, "application/json")
                    });

                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return null;
        }

        private HttpClient CreateHttpClient()
        {
            return new HttpClient
            {
                BaseAddress = new Uri(_configuration.Hmrc.BaseUrl)
            };
        }
    }
}