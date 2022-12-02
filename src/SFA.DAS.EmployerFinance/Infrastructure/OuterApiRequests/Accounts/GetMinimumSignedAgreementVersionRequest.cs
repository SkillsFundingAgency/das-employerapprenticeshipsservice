using SFA.DAS.EmployerFinance.Interfaces.OuterApi;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests.Accounts
{
    public class GetMinimumSignedAgreementVersionRequest : IGetApiRequest
    {
        private readonly long _accountId;
        public string GetUrl => $"accounts/{_accountId}/minimum-signed-agreement-version";

        public GetMinimumSignedAgreementVersionRequest(long accountId)
        {
            _accountId = accountId;
        }
    }
}
