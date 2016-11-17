using System;

namespace SFA.DAS.EAS.Domain.Models.Transaction
{
    public class TransactionLineDetail
    {
        public long SubmissionId { get; set; }
        public string EmpRef { get; set; }
        public decimal EnglishFraction { get; set; }
        public decimal Amount { get;set; }
        public TransactionItemType TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}