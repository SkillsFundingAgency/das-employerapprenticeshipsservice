using System.Collections.Generic;

namespace SFA.DAS.EAS.Account.Api.Types
{
    public class TransactionsViewModel : List<TransactionViewModel>, IAccountResource
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public bool HasPreviousTransactions { get; set; }
        public string PreviousMonthUri { get; set; }
    }
}
