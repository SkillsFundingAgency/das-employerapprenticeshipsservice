using SFA.DAS.EmployerFinance.Models.Transaction;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFinance.Queries.GetAccountTransactions
{
    public class GetAccountTransactionsResponse
    {
        public List<TransactionLine> TransactionLines { get; set; }
    }
}