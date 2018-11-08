using System.Collections.Generic;
using SFA.DAS.EmployerFinance.Models.Transaction;

namespace SFA.DAS.EmployerFinance.Queries.GetAccountTransactionSummary
{
    public class GetAccountTransactionSummaryResponse
    {
        public List<TransactionSummary> Data { get; set; }
    }
}