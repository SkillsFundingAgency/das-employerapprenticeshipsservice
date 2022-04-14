using SFA.DAS.EmployerFinance.Interfaces.OuterApi;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests
{
    public class GetAccountProjectionSummaryRequest : IGetApiRequest
    {
        private readonly long _accountId;
        public string GetUrl => $"Projections/{_accountId}";


        public GetAccountProjectionSummaryRequest(long accountId)
        {
            _accountId = accountId;
        }
    }
}
