using System;

namespace SFA.DAS.EAS.LevyAnalyzer.Models
{
    public enum TransactionItemType : byte
    {
        Unknown = 0,
        Declaration = 1,
        TopUp = 2,
        Payment = 3,
        Transfer = 4
    }

    public class TransactionLine
    {
        public long AccountId { get; set; }
        public DateTime DateCreated { get; set; }
        public long? SubmissionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionItemType TransactionType { get; set; }
        public decimal? LevyDeclared { get; set; }
        public decimal Amount { get; set; }
        public string EmpRef { get; set; }
        public string PeriodEnd { get; set; }
        public long? UkPrn { get; set; }
        public decimal SfaCoInvestmentAmount { get; set; }
        public decimal EmployerCoInvestmentAmount { get; set; }
        public decimal? EnglishFraction { get; set; }
        public long? TransferSenderAccountId { get; set; }
        public string TransferSenderAccountName { get; set; }
        public long? TransferReceiverAccountId { get; set; }
        public string TransferReceiverAccountName { get; set; }
    }
}