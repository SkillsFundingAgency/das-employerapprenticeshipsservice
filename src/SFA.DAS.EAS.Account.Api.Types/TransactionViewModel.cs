using System;

namespace SFA.DAS.EAS.Account.Api.Types
{
    public class TransactionViewModel : IAccountResource
    {
        public string HashedAccountId { get; set; }
        public DateTime DateCreated { get; set; }
        public long? SubmissionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; } 
        public decimal LevyDeclared { get; set; }
        public decimal Amount { get; set; }
        public string PayeSchemeRef { get; set; }
        public string PeriodEnd { get; set; }
        public long UkPrn { get; set; }
    }
}
