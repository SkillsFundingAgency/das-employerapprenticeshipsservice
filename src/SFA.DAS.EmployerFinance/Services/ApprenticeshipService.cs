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
using SFA.DAS.EmployerFinance.Models.Apprenticeships;

namespace SFA.DAS.EmployerFinance.Services
{
    public class ApprenticeshipService : IApprenticeshipService
    {
        private readonly IApiClient _apiClient;

        public ApprenticeshipService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IEnumerable<ApprenticeshipDetail>> GetApprenticeshipsFor(long accountId)
        {
            var applicationsResponse = await _apiClient.Get<GetApplicationsResponse>(new GetApplicationsRequest(accountId, 500)).ConfigureAwait(false);

            return applicationsResponse.Apprenticeships;
        }
    }
}
