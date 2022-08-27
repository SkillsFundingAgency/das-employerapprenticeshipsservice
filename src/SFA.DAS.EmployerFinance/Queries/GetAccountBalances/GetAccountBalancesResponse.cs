using SFA.DAS.EmployerFinance.Models.Account;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFinance.Queries.GetAccountBalances
{
    public class GetAccountBalancesResponse
    {
        public List<AccountBalance> Accounts { get; set; }
    }
}
