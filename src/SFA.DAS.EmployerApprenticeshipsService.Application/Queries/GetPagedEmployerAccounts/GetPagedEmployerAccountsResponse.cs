using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Entities.Account;

namespace SFA.DAS.EAS.Application.Queries.GetPagedEmployerAccounts
{
    public class GetPagedEmployerAccountsResponse
    {
        public List<Account> Accounts { get; set; }
        public int AccountsCount { get; set; }
    }
}