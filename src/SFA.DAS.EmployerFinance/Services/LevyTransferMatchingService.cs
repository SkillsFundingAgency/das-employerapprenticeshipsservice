namespace SFA.DAS.EmployerFinance.Services
{
    using System.Threading.Tasks;
    using SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests;
    using SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses;
    using SFA.DAS.EmployerFinance.Interfaces.OuterApi;

    public class LevyTransferMatchingService : ILevyTransferMatchingService
    {
        private readonly IApiClient _apiClient;

        public LevyTransferMatchingService(
            IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<int> GetPledgesCount(long accountId)
        {
            var getPledgesResponse = await _apiClient.Get<GetPledgesResponse>(new GetPledgesRequest(accountId));

            return getPledgesResponse.TotalPledges;
        }

        public async Task<int> GetApplicationsCount(long accountId)
        {
            //var getApplicationsResponse = await _apiClient.Get<GetApplicationsResponse>(new GetApplicationsRequest(accountId));

            //return getApplicationsResponse.ApplicationsCount;
            return 2;
        }
    }
}