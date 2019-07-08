using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.EAS.Portal.Client.Exceptions;
using SFA.DAS.EAS.Portal.Client.Services.DasRecruit.Models;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using RecruitApiClientConfiguration = SFA.DAS.EAS.Portal.Client.Configuration.RecruitApiClientConfiguration;

namespace SFA.DAS.EAS.Portal.Client.Services.DasRecruit
{
    internal class DasRecruitService : IDasRecruitService
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

        public async Task<IEnumerable<Vacancy>> GetVacancies(long accountId)
        {
            _logger.LogInformation($"Getting Vacancies Summary for account ID: {accountId}");

            var vacanciesSummaryUrl = _apiClientConfiguration.ApiBaseUrl + $"/api/vacancies/?employerAccountId={accountId}&pageSize=1000";

            try
            {
                var vsJson = await _httpClient.GetStringAsync(vacanciesSummaryUrl);
                var vacanciesSummary = JsonConvert.DeserializeObject<VacanciesSummary>(vsJson);
                return vacanciesSummary.Vacancies.Select(Map);
            }
            catch (HttpException ex)
            {
                //todo: we don't want all this!
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
        
        private Vacancy Map(VacancySummary summary)
        {
            return new Vacancy
            {
                ClosingDate = summary.ClosingDate,
                ManageVacancyUrl = summary.RaaManageVacancyUrl,
                NumberOfApplications = summary.NoOfNewApplications+summary.NoOfSuccessfulApplications+summary.NoOfUnsuccessfulApplications,
                Reference = summary.VacancyReference,
                Status = StringToStatus(summary.Status),
                Title = summary.Title,
                TrainingTitle = summary.TrainingTitle
            };
        }

        //todo: no need for all this
        private VacancyStatus StringToStatus(string summaryStatus)
        {
            var status = VacancyStatus.None;

            switch (summaryStatus)
            {
                case "Live":
                    status = VacancyStatus.Live;
                    break;
                case "Closed":
                    status = VacancyStatus.Closed;
                    break;
                case "Rejected":
                    status = VacancyStatus.Rejected;
                    break;
                case "Draft":
                    status = VacancyStatus.Draft;
                    break;
                case "PendingReview":
                    status = VacancyStatus.PendingReview;
                    break;
                default:
                    status = VacancyStatus.None;
                    break;
            }

            return status;
        }
    }
}
