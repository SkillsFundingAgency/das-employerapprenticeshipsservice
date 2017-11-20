using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("agreement_created")]
    public class AgreementCreatedMessage : Message
    {
        public string OrganisationName { get; }
        public long AgreementId { get; }
        public long LegalEntityId { get; }
        public string CreatedByName { get; }

        public AgreementCreatedMessage()
        { }

        public AgreementCreatedMessage(long accountId, long aggreementId, string organisationName, string createdByName, long legalEntityId) : base(accountId)
        {
            AgreementId = aggreementId;
            OrganisationName = organisationName;
            LegalEntityId = legalEntityId;
            CreatedByName = createdByName;
        }
    }
}
