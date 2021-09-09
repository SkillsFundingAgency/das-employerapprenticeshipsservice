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
        private readonly IApiClient _apiClient;

        public CohortsService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<int> GetCohortsCount(long accountId)
        {
            var cohortsCountResponse = await _apiClient.Get<GetCohortsResponse>(new GetCohortsRequest(accountId)).ConfigureAwait(false);

            return cohortsCountResponse.Cohorts.Count;
        }
    }
}
