using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Helpers;
using SFA.DAS.EAS.Application.Http;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi.Http;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Application.Services.EmployerFinanceApi
{
    public class EmployerFinanceApiService : IEmployerFinanceApiService
    {
        private readonly ILog _log;
        private readonly HttpClient _httpClient;

        public EmployerFinanceApiService(IEmployerFinanceApiHttpClientFactory employerFinanceApiHttpClientFactory, ILog log)
        {
            _httpClient = employerFinanceApiHttpClientFactory.CreateHttpClient();
            _log = log;
        }

        public async Task<dynamic> Redirect(string url, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new RestHttpClientException(response, content);

            var x = Json.Decode(content);
            return x;
        }
    }
}