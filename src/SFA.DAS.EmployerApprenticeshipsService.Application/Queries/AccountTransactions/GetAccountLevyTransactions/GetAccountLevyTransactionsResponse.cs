using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountLevyTransactions
{
    /// <summary>
    ///  AML-2454: Move to finance
    /// </summary>
    public class GetAccountLevyTransactionsResponse
    {
        public List<TransactionLine> Transactions { get; set; }
    }
}

