using SFA.DAS.EAS.Domain.Data.Entities.Account;

namespace SFA.DAS.EAS.Application.Queries.GetUserAccounts
{
    public class GetUserAccountsQueryResponse
    {
        public Accounts<Domain.Data.Entities.Account.Account> Accounts { get; set; }
    }
}