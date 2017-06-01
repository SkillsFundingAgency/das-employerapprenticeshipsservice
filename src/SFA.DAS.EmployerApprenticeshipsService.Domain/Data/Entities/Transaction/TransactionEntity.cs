using System;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Domain.Data.Entities.Transaction
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
        public string PayrollYear { get; set; }
        public int PayrollMonth { get; set; }

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
        public int? CourseLevel { get; set; }
        public string PathwayName { get; set; }
        public int? PathwayCode { get; set; }
        public DateTime? CourseStartDate { get; set; }
        public string ApprenticeName { get; set; }
        public string ApprenticeNINumber { get; set; }
        public decimal SfaCoInvestmentAmount { get; set; }
        public decimal EmployerCoInvestmentAmount { get; set; }

    }
}
