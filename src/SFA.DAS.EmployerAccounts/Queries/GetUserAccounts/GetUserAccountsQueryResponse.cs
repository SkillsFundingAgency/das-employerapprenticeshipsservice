using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.Queries.GetUserAccounts
{
    public class GetUserAccountsQueryResponse
    {
        public Accounts<Account> Accounts { get; set; }
    }
}