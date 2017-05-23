using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountLevyTransactions
{
    public class GetAccountLevyTransactionsResponse
    {
        public List<TransactionLine> Transactions { get; set; }
    }
}

