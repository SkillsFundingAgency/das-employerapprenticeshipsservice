using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Queries.GetUserAccounts
{
    public class GetUserAccountsQueryResponse
    {
        //note: account has diverged/forked from domain into accounts
        // and domain account has been continued to be used & updated with api related changes (e.g. ApprenticeshipEmployerType was added)
        // and accounts account has been used & updated for web (presumably) (e.g. with the transfer methods)
        public Accounts<Account> Accounts { get; set; }
    }
}