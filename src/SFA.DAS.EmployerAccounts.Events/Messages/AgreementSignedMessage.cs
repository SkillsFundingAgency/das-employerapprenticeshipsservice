using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("agreement_signed")]
    public class AgreementSignedMessage : Message
    {
        public string OrganisationName { get; }
        public long AgreementId { get; }
        public long LegalEntityId { get; }
        public string SignedByName { get; }

        public AgreementSignedMessage(long accountId, long aggreementId, string organisationName, string signedByName, long legalEntityId) :base(accountId)
        {
            AgreementId = aggreementId;
            OrganisationName = organisationName;
            LegalEntityId = legalEntityId;
            SignedByName = signedByName;
        }
    }
}
