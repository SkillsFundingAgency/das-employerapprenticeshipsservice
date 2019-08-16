using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi.Http;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Application.Services.EmployerAccountsApi
{
    public class EmployerAccountsApiService
    {
        private readonly ILog _log;
        private readonly HttpClient _httpClient;

        public EmployerAccountsApiService(IEmployerAccountsApiHttpClientFactory employerAccountsApiHttpClientFactory, ILog log)
        {
            _log = log;
            //todo: using RestHttpClient would be better, but would need to upgrade the api, which might be a bit much for this story!?
            _httpClient = employerAccountsApiHttpClientFactory.CreateHttpClient();
        }

        //        public async Task<Statistics> GetStatistics(CancellationToken cancellationToken = default)
        public async Task GetStatistics(CancellationToken cancellationToken = default)
        {
            _log.Info($"Getting statistics");

            var response = await _httpClient.GetAsync("/api/statistics", cancellationToken);

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            throw new NotImplementedException();
            //if (!response.IsSuccessStatusCode)
            //    throw new RestHttpClientException(response, content);

            //return JsonConvert.DeserializeObject<Statistics>(content);
        }
    }
}
