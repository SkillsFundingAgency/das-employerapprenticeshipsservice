using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.EAS.Portal.Client.Http;
using SFA.DAS.EAS.Portal.Client.Services.DasRecruit.Models;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Portal.Client.Services.DasRecruit
{
    /// <summary>
    /// https://skillsfundingagency.atlassian.net/wiki/spaces/RAAV2/pages/200245289/Environments
    /// </summary>
    internal class DasRecruitService : IDasRecruitService
    {
        private readonly ILog _log;
        private readonly HttpClient _httpClient;
        private readonly Type _vacancyStatusType;

        //todo: add eas compatible logging
        public DasRecruitService(IRecruitApiHttpClientFactory recruitApiHttpClientFactory, ILog log)
        {
            _log = log;
            //todo: think through lifetimes
            _httpClient = recruitApiHttpClientFactory.CreateHttpClient();
            _vacancyStatusType = typeof(VacancyStatus);
        }

        public async Task<IEnumerable<Vacancy>> GetVacancies(
            string publicHashedAccountId,
            int maxVacanciesToGet = int.MaxValue,
            CancellationToken cancellationToken = default)
        {
            _log.Info($"Getting max {maxVacanciesToGet} VacanciesSummary for account ID {publicHashedAccountId}");

            string vacanciesSummaryUri = $"/api/vacancies/?employerAccountId={publicHashedAccountId}&pageSize={maxVacanciesToGet}";

            try
            {
                var response = await _httpClient.GetAsync(vacanciesSummaryUri, cancellationToken).ConfigureAwait(false);
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                    throw new RestHttpClientException(response, content);

                var vacanciesSummary = JsonConvert.DeserializeObject<VacanciesSummary>(content);
                return vacanciesSummary.Vacancies.Select(Map);
            }
            catch (Exception ex)
            {
                _log.Warn($"Ignoring failed call to recruit API: {ex}");
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
