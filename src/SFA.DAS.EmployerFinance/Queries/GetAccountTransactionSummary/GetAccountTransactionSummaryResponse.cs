using SFA.DAS.EmployerFinance.Models.Transaction;

namespace SFA.DAS.EmployerFinance.Queries.GetAccountTransactionSummary
{
    public class GetAccountTransactionSummaryResponse
    {
        public TransactionSummary[] Data { get; set; }
    }
}