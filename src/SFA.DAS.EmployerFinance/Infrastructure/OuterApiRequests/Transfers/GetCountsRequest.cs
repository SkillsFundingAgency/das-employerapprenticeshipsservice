using SFA.DAS.EmployerFinance.Interfaces.OuterApi;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests.Transfers
{
    public class GetCountsRequest : IGetApiRequest
    {
        private readonly long _accountId;

        public GetCountsRequest(long accountId)
        {
            _accountId = accountId;
        }

        public string GetUrl => $"transfers/{_accountId}/counts";
    }
}