using System;

namespace SFA.DAS.EAS.Domain.Models.Levy
{
    public class TransactionLineDetail
    {
        public long SubmissionId { get; set; }
        public string EmpRef { get; set; }
        public decimal EnglishFraction { get; set; }
        public decimal Amount { get;set; }
        public LevyItemType TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}