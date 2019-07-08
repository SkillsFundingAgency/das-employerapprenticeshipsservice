using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.EAS.Portal.Client.Exceptions;
using SFA.DAS.EAS.Portal.Client.Http;
using SFA.DAS.EAS.Portal.Client.Services.DasRecruit.Models;
using SFA.DAS.EAS.Portal.Client.Types;

namespace SFA.DAS.EAS.Portal.Client.Services.DasRecruit
{
    internal class DasRecruitService : IDasRecruitService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DasRecruitService> _logger;

        public DasRecruitService(
            RecruitApiHttpClientFactory recruitApiHttpClientFactory,
            ILogger<DasRecruitService> logger)
        {
            //todo: think through lifetimes
            _httpClient = recruitApiHttpClientFactory.CreateHttpClient();
            _logger = logger;
        }

        public async Task<IEnumerable<Vacancy>> GetVacancies(long accountId, int maxVacanciesToGet = int.MaxValue)
        {
            _logger.LogInformation($"Getting Vacancies Summary for account ID: {accountId}");

            //todo: we only need a max of 2 vacancies at the minute, as if there are >1, we don't show any details
            // but we do need to know that there is >1
            var vacanciesSummaryUrl = $"/api/vacancies/?employerAccountId={accountId}&pageSize={maxVacanciesToGet}";

            try
            {
                var vsJson = await _httpClient.GetStringAsync(vacanciesSummaryUrl);
                var vacanciesSummary = JsonConvert.DeserializeObject<VacanciesSummary>(vsJson);
                return vacanciesSummary.Vacancies.Select(Map);
            }
            catch (HttpException ex)
            {
                //todo: we don't want all this! consumer all errors. use generic error text
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
