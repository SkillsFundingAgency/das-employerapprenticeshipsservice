namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    public class LegalEntityRemovedMessage
    {
        public long AccountId { get; set; }
        public long LegalEntityId { get; set; }
        public long AgreementId { get; set; }
        public bool AgreementSigned { get; set; }
    }
}
