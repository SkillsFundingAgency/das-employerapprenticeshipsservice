using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("agreement_signed")]
    public class AgreementSignedMessage : AccountMessageBase
    {
        public string OrganisationName { get; }
        public long AgreementId { get; }
        public long LegalEntityId { get; }

        public AgreementSignedMessage()
        { }

        public AgreementSignedMessage(long accountId, long aggreementId, string organisationName, long legalEntityId, string createdBy) 
            :base(accountId, createdBy)
        {
            AgreementId = aggreementId;
            OrganisationName = organisationName;
            LegalEntityId = legalEntityId;
        }
    }
}
