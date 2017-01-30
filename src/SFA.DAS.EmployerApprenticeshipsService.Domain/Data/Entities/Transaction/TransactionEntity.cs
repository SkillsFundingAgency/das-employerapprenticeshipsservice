using System;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Domain.Entities.Transaction
{
    public class TransactionEntity
    {
        //Generic transaction field
        public long AccountId { get; set; }
        public TransactionItemType TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime DateCreated { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public decimal LineAmount { get; set; }

        //Levy Declaration fields
        public long SubmissionId { get; set; }
        public string EmpRef { get; set; }
        public decimal EnglishFraction { get; set; }
        public decimal TopUp { get; set; }

        //Provider Payment fields
        public long UkPrn { get; set; }
        public string PeriodEnd { get; set; }
        public string ProviderName { get; set; }
        public string CourseName { get; set; }
    }
}
