using SFA.DAS.EAS.Domain.Models.Account;

namespace SFA.DAS.EAS.Application.Queries.GetUserAccounts
{
    public class GetUserAccountsQueryResponse
    {
        public Accounts<Domain.Models.Account.Account> Accounts { get; set; }
    }
}