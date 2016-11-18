using System;

namespace SFA.DAS.EAS.Domain.Models.Transaction
{
    public class TransactionDetailSummary
    {
        public string Empref { get; set; }

        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; }
        public decimal EnglishFraction { get; set; }
        public decimal TopUp { get; set; }
        public decimal LineAmount { get; set; }
    }
}