using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.EAS.Portal.Client.Http;
using SFA.DAS.EAS.Portal.Client.Services.Recruit.Http;
using SFA.DAS.EAS.Portal.Client.Services.Recruit.Models;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Portal.Client.Services.Recruit
{
    /// <summary>
    /// https://skillsfundingagency.atlassian.net/wiki/spaces/RAAV2/pages/200245289/Environments
    /// </summary>
    internal class RecruitService : IRecruitService
    {
        private readonly ILog _log;
        private readonly HttpClient _httpClient;
        private readonly Type _vacancyStatusType;

        public RecruitService(IRecruitApiHttpClientFactory recruitApiHttpClientFactory, ILog log)
        {
            _log = log;
            _httpClient = recruitApiHttpClientFactory.CreateHttpClient();
            _vacancyStatusType = typeof(VacancyStatus);
        }

        public async Task<IEnumerable<Vacancy>> GetVacancies(
            string hashedAccountId,
            int maxVacanciesToGet = int.MaxValue,
            CancellationToken cancellationToken = default)
        {
            _log.Info($"Getting max {maxVacanciesToGet} VacanciesSummary for account ID {hashedAccountId}");

            string vacanciesSummaryUri = $"/api/vacancies/?employerAccountId={hashedAccountId}&pageSize={maxVacanciesToGet}";

            try
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var response = await _httpClient.GetAsync(vacanciesSummaryUri, cancellationToken).ConfigureAwait(false);
                // log when we get the response, which will help inform a sensible timeout value
                // (the recruit api in test is taking up to 3.5 seconds, which really slows down the homepage rendering :-( )
                _log.Info($"Received {(int)response.StatusCode} response in {stopWatch.Elapsed}");
                
                if (response.StatusCode == HttpStatusCode.NotFound)
                    return Enumerable.Empty<Vacancy>();

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
                TrainingTitle = summary.TrainingTitle,
                ApplicationMethod = summary.ApplicationMethod
            };
        }
    }
}
