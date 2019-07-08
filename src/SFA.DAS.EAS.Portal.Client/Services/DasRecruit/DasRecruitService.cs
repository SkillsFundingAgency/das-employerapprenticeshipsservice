using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.EAS.Portal.Client.Exceptions;
using SFA.DAS.EAS.Portal.Client.Services.DasRecruit.Models;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using RecruitApiClientConfiguration = SFA.DAS.EAS.Portal.Client.Configuration.RecruitApiClientConfiguration;

namespace SFA.DAS.EAS.Portal.Client.Services.DasRecruit
{
    public class DasRecruitService : IDasRecruitService
    {
        private readonly HttpClient _httpClient;
        private readonly RecruitApiClientConfiguration _apiClientConfiguration;
        private readonly ILogger _logger;

        public DasRecruitService(RecruitApiClientConfiguration apiClientConfiguration,
            ILogger logger)
        {
            _apiClientConfiguration = apiClientConfiguration;
            _logger = logger;
            _httpClient = new HttpClientBuilder()
                .WithDefaultHeaders()
                .WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(_apiClientConfiguration))
                .Build();
        }

        public async Task<VacanciesSummary> GetVacanciesSummary(long accountId)
        {
            _logger.LogInformation($"Getting Vacancies Summary for account ID: {accountId}");

            var vacanciesSummaryUrl = _apiClientConfiguration.ApiBaseUrl + $"/api/vacancies/?employerAccountId={accountId}&pageSize=1000";

            try
            {
                var vsJson = await _httpClient.GetStringAsync(vacanciesSummaryUrl);
                return JsonConvert.DeserializeObject<VacanciesSummary>(vsJson);
            }
            catch (HttpException ex)
            {
                switch (ex.StatusCode)
                {
                    case 400:
                        _logger.LogError(ex, $"Bad request sent to recruit API for account ID: {accountId}");
                        break;
                    case 404:
                        _logger.LogError(ex,$"Resource not found - recruit API for account ID: {accountId}");
                        break;
                    case 408:
                        _logger.LogError(ex, $"Request sent to recruit API for account ID: {accountId} timed out");
                        break;
                    case 429:
                        _logger.LogError(ex, $"Too many requests sent to recruit API for account ID: {accountId}");
                        break;
                    case 500:
                        _logger.LogError(ex, $"Recruit API reported internal Server error for account ID: {accountId}");
                        break;
                    case 503:
                        _logger.LogError(ex, "Recruit API is unavailable");
                        break;

                    default: throw;
                }

                return null;
            }
        }
    }
}
