using SFA.DAS.EmployerFinance.Interfaces.OuterApi;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests
{
    public class GetPledgesRequest : IGetApiRequest
    {
        private readonly long _accountId;

        public GetPledgesRequest(long accountId)
        {
            _accountId = accountId;
        }

        public string GetUrl => $"Pledges?accountId={_accountId}";
    }
}