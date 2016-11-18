using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Domain.Models.Transaction
{
    public class TransactionLine
    {
        public long AccountId { get; set; }
        public string Description { get; set; }
        public TransactionItemType TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public List<TransactionLine> SubTransactions { get; set; }
    }
}
