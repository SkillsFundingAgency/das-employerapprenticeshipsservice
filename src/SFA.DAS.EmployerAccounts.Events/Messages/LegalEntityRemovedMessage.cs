using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("legal_entity_removed")]
    public class LegalEntityRemovedMessage : Message
    {
        public LegalEntityRemovedMessage()
        {

        }

        public LegalEntityRemovedMessage(long accountId,  long aggreementId, bool agreementSigned, string signedByName, long legalEntityId) : base(signedByName, accountId)
        {
            AgreementId = aggreementId;
            AgreementSigned = agreementSigned;
            LegalEntityId = legalEntityId;
        }

        public string CompanyName { get;  }
        public long AgreementId { get; }
        public bool AgreementSigned { get; }
        public long LegalEntityId { get;  }
    }
}
