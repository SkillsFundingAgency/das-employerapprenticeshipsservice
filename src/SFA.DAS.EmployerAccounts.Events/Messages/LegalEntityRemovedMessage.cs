using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("legal_entity_removed")]
    public class LegalEntityRemovedMessage : Message
    {
        public LegalEntityRemovedMessage()
        {

        }

        public LegalEntityRemovedMessage(long accountId, long legalEntityId, long aggreementId, bool agreementSigned)
        {
            AccountId = accountId;
            LegalEntityId = legalEntityId;
            AgreementId = aggreementId;
            AgreementSigned = agreementSigned;
        }

        public string CompanyName { get; set; }
        public long AccountId { get; }
        public long LegalEntityId { get; }
        public long AgreementId { get; }
        public bool AgreementSigned { get; }
    }
}
