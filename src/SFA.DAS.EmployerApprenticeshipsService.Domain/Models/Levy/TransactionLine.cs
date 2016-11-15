using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Domain.Models.Levy
{
    public class TransactionLine
    {
        public string Description { get; set; }
        public long AccountId { get; set; }
        public long SubmissionId { get; set; }
        public Guid? PaymentId { get; set; }
        public DateTime TransactionDate { get; set; }
        public LevyItemType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public string EmpRef { get; set; }
        public string PeriodEnd { get; set; }

        public List<TransactionLine> SubTransactions { get; set; }
    }
}
