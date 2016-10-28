using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Entities.Account;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccounts
{
    public class GetEmployerAccountsResponse
    {
        public List<Account> Accounts { get; set; }
        public int AccountsCount { get; set; }
    }
}