using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("agreement_signed")]
    public class AgreementSignedMessage : AccountMessageBase
    {
        public long LegalEntityId { get; }
        public long AgreementId { get; }
        public bool CohortCreated { get; }

        public AgreementSignedMessage()
        {   }

        public AgreementSignedMessage(long accountId, long agreementId, long legalEntityId, bool cohortCreated, string creatorName, string creatorUserRef) : base(accountId, creatorName, creatorUserRef)
        {
            AgreementId = agreementId;
            LegalEntityId = legalEntityId;
            CohortCreated = cohortCreated;
        }
    }
}
