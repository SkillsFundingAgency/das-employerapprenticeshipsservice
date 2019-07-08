using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.EAS.Portal.Client.Http;
using SFA.DAS.EAS.Portal.Client.Services.DasRecruit.Models;
using SFA.DAS.EAS.Portal.Client.Types;
//using Microsoft.AspNetCore.WebUtilities;

namespace SFA.DAS.EAS.Portal.Client.Services.DasRecruit
{
    /// <summary>
    /// https://skillsfundingagency.atlassian.net/wiki/spaces/RAAV2/pages/200245289/Environments
    /// </summary>
    internal class DasRecruitService : IDasRecruitService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DasRecruitService> _logger;

        public DasRecruitService(
            IRecruitApiHttpClientFactory recruitApiHttpClientFactory,
            ILogger<DasRecruitService> logger)
        {
            //todo: think through lifetimes
            _httpClient = recruitApiHttpClientFactory.CreateHttpClient();
            _logger = logger;
        }

        public async Task<IEnumerable<Vacancy>> GetVacancies(
            long accountId,
            int maxVacanciesToGet = int.MaxValue,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Getting Vacancies Summary for account ID: {accountId}");

            //todo: we only need a max of 2 vacancies at the minute, as if there are >1, we don't show any details
            // but we do need to know that there is >1
            string vacanciesSummaryUri = $"/api/vacancies/?employerAccountId={accountId}&pageSize={maxVacanciesToGet}";

            try
            {
                var response = await _httpClient.GetAsync(vacanciesSummaryUri, cancellationToken).ConfigureAwait(false);
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                    throw new RestHttpClientException(response, content);

                // not worth adding another dependency on Microsoft.AspNet.WebApi.Client for this 1 method
                //var vacanciesSummary = await response.Content.ReadAsAsync<VacanciesSummary>(cancellationToken).ConfigureAwait(false);

                var vacanciesSummary = JsonConvert.DeserializeObject<VacanciesSummary>(content);
                return vacanciesSummary.Vacancies.Select(Map);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Ignoring failed call to recruit API: {ex}");
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
