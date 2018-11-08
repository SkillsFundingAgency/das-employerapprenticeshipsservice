using System.Collections.Generic;

namespace SFA.DAS.EmployerFinance.Models.Transaction
{
    public class TransactionsViewModel : List<TransactionViewModel>, EmployerFinance.Models.IAccountResource
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public bool HasPreviousTransactions { get; set; }
        public string PreviousMonthUri { get; set; }
    }
}
