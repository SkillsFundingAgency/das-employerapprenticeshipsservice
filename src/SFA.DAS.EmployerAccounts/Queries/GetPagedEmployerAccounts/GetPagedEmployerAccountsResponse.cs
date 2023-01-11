using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Queries.GetPagedEmployerAccounts;

public class GetPagedEmployerAccountsResponse
{
    public List<Account> Accounts { get; set; }
    public int AccountsCount { get; set; }
}