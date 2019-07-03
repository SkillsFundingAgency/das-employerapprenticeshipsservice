using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.EAS.DasRecruitService.Models;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;

namespace SFA.DAS.EAS.DasRecruitService.Services
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

        public async Task<VacanciesSummary> GetVacanciesSummary(long accountId, long legalEntityId, long ukprn)
        {
            _logger.LogInformation($"Getting Vacancies Summary for account ID: {accountId}");

            var vacanciesSummaryUrl = $"/api/vacancies/?employerAccountId={accountId}&legalEntityId={legalEntityId}&ukprn={ukprn}&pageSize=25&pageNo=1";

            try
            {
                var vacanciesSummaryResult =
                    await _httpClient.GetStringAsync(vacanciesSummaryUrl);

                var vacanciesSummary = JsonConvert.DeserializeObject<VacanciesSummary>(vacanciesSummaryResult);
                return vacanciesSummary;
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
