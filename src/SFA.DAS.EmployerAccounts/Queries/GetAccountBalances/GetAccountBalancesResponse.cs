using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountBalances
{
    public class GetAccountBalancesResponse
    {
        public List<AccountBalance> Accounts { get; set; }
    }
}