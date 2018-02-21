namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    public class LevyDeclarationUpdatedMessage : AccountMessageBase
    {
        public string PayrollYear { get; set; }
        public short? PayrollMonth { get; set; }
        public decimal LevyDeclaredInMonth { get; set; }

        public LevyDeclarationUpdatedMessage() : base(0, null, null) { }
        public LevyDeclarationUpdatedMessage(long accountId, string creatorName, string creatorUserRef)
            : base(accountId, creatorName, creatorUserRef) { }
    }
}