using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("agreement_created")]
    public class AgreementCreatedMessage : AccountMessageBase
    {
        public string OrganisationName { get; }
        public long AgreementId { get; }
        public long LegalEntityId { get; }

        public AgreementCreatedMessage()
        { }

        public AgreementCreatedMessage(long accountId, long aggreementId, string organisationName, long legalEntityId, string creatorName, string creatorUserRef) : base(accountId, creatorName, creatorUserRef)
        {
            AgreementId = aggreementId;
            OrganisationName = organisationName;
            LegalEntityId = legalEntityId;
        }
    }
}
