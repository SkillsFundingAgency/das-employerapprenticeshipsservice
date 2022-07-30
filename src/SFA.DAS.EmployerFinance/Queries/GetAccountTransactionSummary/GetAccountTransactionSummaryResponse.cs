using SFA.DAS.EmployerFinance.Models.Transaction;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFinance.Queries.GetAccountTransactionSummary
{
    public class GetAccountTransactionSummaryResponse
    {
        public List<TransactionSummary> Data { get; set; }
    }
}
