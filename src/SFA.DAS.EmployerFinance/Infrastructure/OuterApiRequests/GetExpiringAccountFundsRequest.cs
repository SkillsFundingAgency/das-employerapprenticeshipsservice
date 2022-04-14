using SFA.DAS.EmployerFinance.Interfaces.OuterApi;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests
{
    public class GetExpiringAccountFundsRequest : IGetApiRequest
    {
        private readonly long _accountId;
        public string GetUrl => $"account/{_accountId}/expiring-funds";


        public GetExpiringAccountFundsRequest(long accountId)
        {
            _accountId = accountId;
        }
    }
}
