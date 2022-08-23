using SFA.DAS.EmployerFinance.Interfaces.OuterApi;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests.Transfers
{
    public class GetFinancialBreakdownRequest : IGetApiRequest
    {
        private readonly long _accountId;

        public GetFinancialBreakdownRequest(long accountId)
        {
            _accountId = accountId;
        }

        public string GetUrl => $"transfers/{_accountId}/financial-breakdown";
    }
}
