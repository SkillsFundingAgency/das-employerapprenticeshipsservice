using System;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    public class LevySchemeDeclarationUpdatedMessage : AccountMessageBase
    {
        public long Id { get; set; }
        public string EmpRef { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string PayrollYear { get; set; }
        public short? PayrollMonth { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool EndOfYearAdjustment { get; set; }
        public decimal EndOfYearAdjustmentAmount { get; set; }
        public decimal LevyAllowanceForYear { get; set; }
        public DateTime? DateCeased { get; set; }
        public DateTime? InactiveFrom { get; set; }
        public DateTime? InactiveTo { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal LevyDeclaredInMonth { get; set; }

        public LevySchemeDeclarationUpdatedMessage() : base(0, null, null) { }
        public LevySchemeDeclarationUpdatedMessage(long accountId, string creatorName, string creatorUserRef)
            : base(accountId, creatorName, creatorUserRef) { }
    }
}