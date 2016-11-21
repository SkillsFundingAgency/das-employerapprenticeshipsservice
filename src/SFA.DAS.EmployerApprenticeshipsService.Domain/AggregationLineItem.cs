using System;

namespace SFA.DAS.EAS.Domain
{
    public class AggregationLineItem
    {
        public long Id { get; set; }
        public string EmpRef { get; set; }
        public decimal Amount { get; set; }
        public decimal EnglishFraction { get; set; }
        public DateTime ActivityDate { get; set; }
        public TransactionItemType TransactionItemType { get; set; }
        public decimal CalculatedAmount { get; set; }
        public bool IsLastSubmission { get; set; }
        public decimal LevyDueYtd { get; set; }
        public long AccountId { get; set; }
    }
}