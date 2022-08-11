using SFA.DAS.EmployerFinance.Interfaces.OuterApi;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests.Projections
{
    public class GetAccountProjectionSummaryRequest : IGetApiRequest
    {
        private readonly long _accountId;
        public string GetUrl => $"projections/{_accountId}";

        public GetAccountProjectionSummaryRequest(long accountId)
        {
            _accountId = accountId;
        }
    }
}
