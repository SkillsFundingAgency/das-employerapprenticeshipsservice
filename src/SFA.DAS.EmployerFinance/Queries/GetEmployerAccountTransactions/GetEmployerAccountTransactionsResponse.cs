using SFA.DAS.EmployerFinance.Models.Transaction;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFinance.Queries.GetEmployerAccountTransactions
{
    public class GetEmployerAccountTransactionsResponse
    {
        public AggregationData Data { get; set; }
        public bool AccountHasPreviousTransactions { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
    }

    //public class AggregationData
    //{
    //    public long AccountId { get; set; }
    //    public string HashedAccountId { get; set; }
    //    public ICollection<TransactionLine> TransactionLines { get; set; }
    //}
}