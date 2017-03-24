using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Data.Entities.Account;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances
{
    public class GetAccountBalancesResponse
    {
        public List<AccountBalance> Accounts { get; set; }
    }
}