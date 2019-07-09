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

namespace SFA.DAS.EAS.Portal.Client.Services.DasRecruit
{
    /// <summary>
    /// https://skillsfundingagency.atlassian.net/wiki/spaces/RAAV2/pages/200245289/Environments
    /// </summary>
    internal class DasRecruitService : IDasRecruitService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DasRecruitService> _logger;
        private readonly Type _vacancyStatusType;

        public DasRecruitService(
            IRecruitApiHttpClientFactory recruitApiHttpClientFactory,
            ILogger<DasRecruitService> logger)
        {
            //todo: think through lifetimes
            _httpClient = recruitApiHttpClientFactory.CreateHttpClient();
            _logger = logger;
            _vacancyStatusType = typeof(VacancyStatus);
        }

        public async Task<IEnumerable<Vacancy>> GetVacancies(
            long accountId,
            int maxVacanciesToGet = int.MaxValue,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Getting Vacancies Summary for account ID: {accountId}");

            string vacanciesSummaryUri = $"/api/vacancies/?employerAccountId={accountId}&pageSize={maxVacanciesToGet}";

            try
            {
                //todo: set timeout property, so we don't end up delaying the rendering of the homepage for a misbehaving api
                var response = await _httpClient.GetAsync(vacanciesSummaryUri, cancellationToken).ConfigureAwait(false);
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                    throw new RestHttpClientException(response, content);

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
                NumberOfApplications = summary.NoOfNewApplications + summary.NoOfSuccessfulApplications + summary.NoOfUnsuccessfulApplications,
                Reference = summary.VacancyReference,
                Status = (VacancyStatus)Enum.Parse(_vacancyStatusType, summary.Status, true),
                Title = summary.Title,
                TrainingTitle = summary.TrainingTitle
            };
        }
    }
}
