using SFA.DAS.EmployerFinance.Interfaces.OuterApi;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests.Accounts
{
    public class GetAccountTeamMembersWhichReceiveNotificationsRequest : IGetApiRequest
    {
        private readonly long _accountId;
        public string GetUrl => $"accounts/{_accountId}/users/which-receive-notifications";

        public GetAccountTeamMembersWhichReceiveNotificationsRequest(long accountId)
        {
            _accountId = accountId;
        }
    }
}
