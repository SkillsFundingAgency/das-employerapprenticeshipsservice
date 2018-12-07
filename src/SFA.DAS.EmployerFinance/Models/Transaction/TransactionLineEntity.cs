using System;

namespace SFA.DAS.EmployerFinance.Models.Transaction
{
    public class TransactionLineEntity
    {
        //Generic transaction field
        public long Id { get; set; }
        public long AccountId { get; set; }
        public TransactionItemType TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime DateCreated { get; set; }
        public decimal Amount { get; set; }


        //Levy Declaration fields
        public long? SubmissionId { get; set; }
        public string EmpRef { get; set; }
        public decimal? EnglishFraction { get; set; }


        //Provider Payment fields
        public long? UkPrn { get; set; }
        public string PeriodEnd { get; set; }
        public decimal SfaCoInvestmentAmount { get; set; }
        public decimal EmployerCoInvestmentAmount { get; set; }

        //Transfer fields
        public long? TransferSenderAccountId { get; set; }
        public string TransferSenderAccountName { get; set; }
        public long? TransferReceiverAccountId { get; set; }
        public string TransferReceiverAccountName { get; set; }

    }
}
