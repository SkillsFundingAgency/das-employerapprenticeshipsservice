using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests.Transfers;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses.Transfers;
using SFA.DAS.EmployerFinance.Interfaces.OuterApi;

namespace SFA.DAS.EmployerFinance.Services
{
    public class ManageApprenticeshipsService : IManageApprenticeshipsService
    {
        private readonly IApiClient _apiClient;

        public ManageApprenticeshipsService(
            IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<GetIndexResponse> GetIndex(long accountId)
        {
            return await _apiClient.Get<GetIndexResponse>(new GetIndexRequest(accountId));
        }
    }
}