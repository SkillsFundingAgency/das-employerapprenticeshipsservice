using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.NLog.Logger;
using SFA.DAS.EAS.DasRecruitService.Models;
using SFA.DAS.EAS.Portal.Infrastructure.Configuration;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;

namespace SFA.DAS.EAS.DasRecruitService.Services
{
    public class DasRecruitService : IDasRecruitService
    {
        private readonly HttpClient _httpClient;
        private readonly RecruitApiClientConfiguration _apiClientConfiguration;
        private readonly ILog _logger;

        public DasRecruitService(RecruitApiClientConfiguration apiClientConfiguration,
            ILog logger)
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
            _logger.Info($"Getting Vacancies Summary for account ID: {accountId}");

            var vacanciesSummaryUrl = $"/api/vacancies/?employerAccountId={accountId}&pageSize=25&pageNo=1";

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
                        _logger.Error(ex, $"Bad request sent to recruit API for account ID: {accountId}");
                        break;
                    case 404:
                        _logger.Error(ex,$"Resource not found - recruit API for account ID: {accountId}");
                        break;
                    case 408:
                        _logger.Error(ex, $"Request sent to recruit API for account ID: {accountId} timed out");
                        break;
                    case 429:
                        _logger.Error(ex, $"Too many requests sent to recruit API for account ID: {accountId}");
                        break;
                    case 500:
                        _logger.Error(ex, $"Recruit API reported internal Server error for account ID: {accountId}");
                        break;
                    case 503:
                        _logger.Error(ex, "Recruit API is unavailable");
                        break;

                    default: throw;
                }

                return null;
            }
        }

        public IVacancy MapToVacancy(VacancySummary vacancySummary)
        {
            return new Vacancy
            {
                ClosingDate = vacancySummary.ClosingDate.Value,
                NumberOfApplications = vacancySummary.NoOfNewApplications+vacancySummary.NoOfSuccessfulApplications+vacancySummary.NoOfUnsuccessfulApplications,
                Reference = vacancySummary.VacancyReference.Value,
                Status = StringToStatus(vacancySummary.Status),
                Title = vacancySummary.Title,
                TrainingTitle =  vacancySummary.TrainingTitle,
                ManageVacancyUrl = vacancySummary.RaaManageVacancyUrl
            };
        }

        private VacancyStatus StringToStatus(string statusString)
        {
            VacancyStatus status;
            switch (statusString)
            {
                case "Live":
                    status = VacancyStatus.Live;
                    break;
                case "Closed":
                    status = VacancyStatus.Closed;
                    break;
                case "Draft":
                    status = VacancyStatus.Draft;
                    break;
                case "PendingReview":
                    status = VacancyStatus.PendingReview;
                    break;
                case "Rejected":
                    status = VacancyStatus.Rejected;
                    break;
                default:
                    status = VacancyStatus.None;
                    break;
            }
            return status;
        }
    }

    
}
