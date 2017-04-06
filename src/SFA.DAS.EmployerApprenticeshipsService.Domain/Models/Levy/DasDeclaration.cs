using System;

namespace SFA.DAS.EAS.Domain.Models.Levy
{
    public class DasDeclaration
    {
        public string Id { get; set; }
        public decimal LevyDueYtd { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string SubmissionType { get; set; }
        public decimal LevyAllowanceForFullYear { get; set; }
        public string PayrollYear { get; set; }
        public short? PayrollMonth { get; set; }
        public bool NoPaymentForPeriod { get; set; }
        public DateTime? DateCeased { get; set; }
        public DateTime? InactiveFrom { get; set; }
        public DateTime? InactiveTo { get; set; }
        public bool EndOfYearAdjustment { get; set; }
        public decimal EndOfYearAdjustmentAmount { get; set; }
        public long SubmissionId { get; set; }
    }
}