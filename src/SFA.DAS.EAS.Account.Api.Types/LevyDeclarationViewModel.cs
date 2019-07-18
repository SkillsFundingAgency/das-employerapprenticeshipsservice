using System;

namespace SFA.DAS.EAS.Account.Api.Types
{
    public class LevyDeclarationViewModel : IAccountResource
    {
        public string HashedAccountId { get; set; }
        public long Id { get; set; }
        public string PayeSchemeReference { get; set; }
        public decimal? LevyDueYtd { get; set; }
        public decimal? LevyAllowanceForYear { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public long SubmissionId { get; set; }
        public string PayrollYear { get; set; }
        public short? PayrollMonth { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool EndOfYearAdjustment { get; set; }
        public decimal? EndOfYearAdjustmentAmount { get; set; }
        public DateTime? DateCeased { get; set; }
        public DateTime? InactiveFrom { get; set; }
        public DateTime? InactiveTo { get; set; }
        public long HmrcSubmissionId { get; set; }
        public decimal EnglishFraction { get; set; }
        public decimal TopUpPercentage { get; set; }
        public decimal TopUp { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal LevyDeclaredInMonth { get; set; }
    }
}
