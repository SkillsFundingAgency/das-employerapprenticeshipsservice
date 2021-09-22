using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Interfaces.OuterApi;

namespace SFA.DAS.EmployerFinance.Infrastructure
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ManageApprenticeshipsOuterApiConfiguration _config;

        public ApiClient(
            HttpClient httpClient,
            ManageApprenticeshipsOuterApiConfiguration options)
        {
            _httpClient = httpClient;
            _config = options;
            _httpClient.BaseAddress = new Uri(_config.BaseUrl);
        }

        public async Task<TResponse> Get<TResponse>(IGetApiRequest request)
        {
            AddHeaders();

            var response = await _httpClient.GetAsync(request.GetUrl).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<TResponse>(json);
        }

        private void AddHeaders()
        {
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _config.Key);
            _httpClient.DefaultRequestHeaders.Add("X-Version", "1");
        }
    }
}