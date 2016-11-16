using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Domain.Models.Levy
{
    public class TransactionSummary
    {
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public List<TransactionLine> TransactionLines { get; set; }
        public string Id { get; set; }
    }
}