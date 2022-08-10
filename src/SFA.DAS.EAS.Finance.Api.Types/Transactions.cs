using System.Collections.Generic;

namespace SFA.DAS.EmployerFinance.Api.Types
{
    public class Transactions : List<Transaction>
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public bool HasPreviousTransactions { get; set; }
        public string PreviousMonthUri { get; set; }
    }
}
