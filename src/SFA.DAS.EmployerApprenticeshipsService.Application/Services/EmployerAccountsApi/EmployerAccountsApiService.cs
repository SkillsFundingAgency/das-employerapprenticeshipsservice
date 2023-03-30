using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Newtonsoft.Json;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Http;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi.Http;
using SFA.DAS.EmployerAccounts.Api.Types;
using Microsoft.Extensions.Logging;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Application.Services.EmployerAccountsApi
{
    public class EmployerAccountsApiService : IEmployerAccountsApiService
    {
        private readonly ILogger<EmployerAccountsApiService> _log;
        private readonly HttpClient _httpClient;
      
        public EmployerAccountsApiService(
            IEmployerAccountsApiHttpClientFactory employerAccountsApiHttpClientFactory,
            ILogger<EmployerAccountsApiService> log)
        {
            _log = log;

            //todo: using RestHttpClient would be better, but would need to upgrade the api, which might be a bit much for this story!?
            _httpClient = employerAccountsApiHttpClientFactory.CreateHttpClient();
        }

        public async Task<Statistics> GetStatistics(CancellationToken cancellationToken = default(CancellationToken))
        {
            _log.LogInformation($"Getting statistics");

            var response = await _httpClient.GetAsync("/api/statistics", cancellationToken).ConfigureAwait(false);

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new RestHttpClientException(response, content);

            return JsonConvert.DeserializeObject<Statistics>(content);
        }

        public async Task<PagedApiResponseViewModel<AccountWithBalanceViewModel>> GetAccounts(string toDate = null, int pageSize = 1000, int pageNumber = 1, CancellationToken cancellationToken = default(CancellationToken))
        {
            _log.LogInformation($"Getting paged accounts");

            var response = await _httpClient.GetAsync($"/api/accounts?{(string.IsNullOrWhiteSpace(toDate) ? "" : "toDate=" + toDate + "&")}pageNumber={pageNumber}&pageSize={pageSize}", cancellationToken).ConfigureAwait(false); 

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new RestHttpClientException(response, content);

            return JsonConvert.DeserializeObject<PagedApiResponseViewModel<AccountWithBalanceViewModel>>(content);
        }

        public async Task<AccountDetailViewModel> GetAccount(string hashedAccountId, CancellationToken cancellationToken = default(CancellationToken))
        {
            _log.LogInformation($"Getting paged accounts");

            var response = await _httpClient.GetAsync($"/api/accounts/{hashedAccountId}", cancellationToken).ConfigureAwait(false);

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new RestHttpClientException(response, content);

            return JsonConvert.DeserializeObject<AccountDetailViewModel>(content);
        }

        public async Task<string> Redirect(string url, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new RestHttpClientException(response, content);

            return content;
        }
    }
}
