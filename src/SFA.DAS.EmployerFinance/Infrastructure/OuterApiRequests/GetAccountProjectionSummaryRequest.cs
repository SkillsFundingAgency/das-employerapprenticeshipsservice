using SFA.DAS.EmployerFinance.Interfaces.OuterApi;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests
{
    public class GetAccountProjectionSummaryRequest : IGetApiRequest
    {
        private readonly long _accountId;
        public string GetUrl => $"account/{_accountId}/account-projection";


        public GetAccountProjectionSummaryRequest(long accountId)
        {
            _accountId = accountId;
        }
    }
}
