using System.Collections.Generic;
using SFA.DAS.EmployerFinance.Models.Transaction;

namespace SFA.DAS.EmployerFinance.Queries.GetAccountLevyTransactions
{
    public class GetAccountLevyTransactionsResponse
    {
        public List<TransactionLine> Transactions { get; set; }
    }
}

