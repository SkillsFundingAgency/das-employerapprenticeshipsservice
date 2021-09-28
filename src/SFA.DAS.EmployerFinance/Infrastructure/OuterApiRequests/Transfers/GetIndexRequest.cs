using SFA.DAS.EmployerFinance.Interfaces.OuterApi;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests.Transfers
{
    public class GetIndexRequest : IGetApiRequest
    {
        private readonly long _accountId;

        public GetIndexRequest(long accountId)
        {
            _accountId = accountId;
        }

        public string GetUrl => $"Transfers/{_accountId}";
    }
}