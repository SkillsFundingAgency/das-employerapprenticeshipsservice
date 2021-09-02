using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses;
using SFA.DAS.EmployerFinance.Interfaces.OuterApi;
using SFA.DAS.EmployerFinance.Models.Cohort;

namespace SFA.DAS.EmployerFinance.Services
{
    public class CohortsService : ICohortsService
    {
        private readonly HttpClient _httpClient;

        public CohortsService(HttpClient httpClient, string cohortsBaseUrl)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(cohortsBaseUrl);
        }

        public async Task<int> GetCohortsCount(long accountId)
        {
            var cohortsCountResponse = await _httpClient.GetAsync(new GetCohortsRequest(accountId).GetUrl).ConfigureAwait(false);

            cohortsCountResponse.EnsureSuccessStatusCode();
            var json = await cohortsCountResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            var currentCohorts = JsonConvert.DeserializeObject<CurrentCohorts>(json);

            return currentCohorts.Cohorts.Count;
        }
    }
}
