using Newtonsoft.Json;
using SFA.DAS.EAS.Finance.Api.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Api.Client
{
    public class EmployerFinanceApiClient : IEmployerFinanceApiClient
    {
        private readonly IEmployerFinanceApiClientConfiguration _configuration;
        private readonly SecureHttpClient _httpClient;

        public EmployerFinanceApiClient(IEmployerFinanceApiClientConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new SecureHttpClient(configuration);
        }

        public Task HealthCheck()
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}/api/healthcheck";

            return _httpClient.GetAsync(url);
        }

        public async Task<ICollection<LevyDeclarationViewModel>> GetLevyDeclarations(string hashedAccountId)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/accounts/{hashedAccountId}/levy";
            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<List<LevyDeclarationViewModel>>(json);
        }

        private string GetBaseUrl()
        {
            return _configuration.ApiBaseUrl.Trim('/');
        }
    }
}