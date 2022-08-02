using SFA.DAS.EmployerFinance.Interfaces.OuterApi;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests.Projections
{
    public class GetAccountTeamMembersWhichReceiveNotifications : IGetApiRequest
    {
        private readonly long _accountId;
        public string GetUrl => $"accounts/{_accountId}/users/which-receive-notifications";

        public GetAccountTeamMembersWhichReceiveNotifications(long accountId)
        {
            _accountId = accountId;
        }
    }
}
