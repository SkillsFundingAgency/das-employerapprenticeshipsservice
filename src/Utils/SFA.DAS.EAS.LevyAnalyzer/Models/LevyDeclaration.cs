using System;

namespace SFA.DAS.EAS.LevyAnalyser.Models
{
    public class LevyDeclaration
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string EmpRef { get; set; }
        public decimal? LevyDueYTD { get; set; }
        public decimal? LevyAllowanceForYear { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public long SubmissionId { get; set; }
        public string PayrollYear { get; set; }
        public byte? PayrollMonth { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool EndOfYearAdjustment { get; set; }
        public decimal? EndOfYearAdjustmentAmount { get; set; }
        public DateTime? DateCeased { get; set; }
        public DateTime? InactiveFrom { get; set; }
        public DateTime? InactiveTo { get; set; }
        public bool? NoPaymentForPeriod { get; set; }
    }
}