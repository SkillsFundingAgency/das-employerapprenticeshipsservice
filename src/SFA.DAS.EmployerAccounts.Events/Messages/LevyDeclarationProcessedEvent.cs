using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("levy_declaration_processed")]
    public class LevyDeclarationProcessedEvent: AccountMessageBase
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

        public LevyDeclarationProcessedEvent(): base(0, null, null) { }
        public LevyDeclarationProcessedEvent(long accountId, string creatorName, string creatorUserRef)
            : base(accountId, creatorName, creatorUserRef) { }
    }
}