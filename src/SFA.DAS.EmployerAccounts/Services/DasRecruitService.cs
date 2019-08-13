using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.EAS.Portal.Client.Http;
using SFA.DAS.EmployerAccounts.Features;
using SFA.DAS.EmployerAccounts.Models.Recruit;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class DasRecruitService : IDasRecruitService
    {
        private readonly ILog _log;
        private readonly HttpClient _httpClient;

        public DasRecruitService(IRecruitApiHttpClientFactory recruitApiHttpClientFactory, ILog log)
        {
            _log = log;
            _httpClient = recruitApiHttpClientFactory.CreateHttpClient();
        }

        public async Task<VacanciesSummary> GetVacanciesByLegalEntity(string hashedAccountId, long legalEntityId)
        {
            _log.Info($"Getting VacanciesSummary for account ID {hashedAccountId} and legalEntityId {legalEntityId}");

            string vacanciesSummaryUri =
                $"/api/vacancies/?employerAccountId={hashedAccountId}&legalEntityId={legalEntityId}";
            try
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var response = await _httpClient.GetAsync(vacanciesSummaryUri).ConfigureAwait(false);
                _log.Info($"Received {(int)response.StatusCode} response in {stopWatch.Elapsed}");

                if (response.StatusCode == HttpStatusCode.NotFound)
                    return new VacanciesSummary(null,0,0,0,0);

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                    throw new RestHttpClientException(response, content);

                return JsonConvert.DeserializeObject<VacanciesSummary>(content);
            }
            catch (Exception ex)
            {
                _log.Warn($"Ignoring failed call to recruit API: {ex}");
                return null;
            }
        }
    }
}
