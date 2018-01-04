using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("legal_entity_removed")]
    public class LegalEntityRemovedMessage : AccountMessageBase
    {
        public long AgreementId { get; }
        public bool AgreementSigned { get; }
        public long LegalEntityId { get; }

        public string LegalEntityName { get; set; }
       
        public LegalEntityRemovedMessage()
        { }

        public LegalEntityRemovedMessage(long accountId,  long aggreementId, bool agreementSigned, long legalEntityId, string legalEntityName, string creatorName, string creatorUserRef) 
            : base(accountId, creatorName, creatorUserRef)
        {
            AgreementId = aggreementId;
            AgreementSigned = agreementSigned;
            LegalEntityId = legalEntityId;
            LegalEntityName = legalEntityName;
        }
    }
}
