using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("agreement_signed")]
    public class AgreementSignedMessage : Message
    {
        public string ProviderName { get; }
        public long AgreementId { get; }
        public long LegalEntityId { get; }
        public string SignedByName { get; }

        public AgreementSignedMessage(long accountId, long aggreementId, string providerName, string signedByName, long legalEntityId) :base(accountId)
        {
            AgreementId = aggreementId;
            ProviderName = providerName;
            LegalEntityId = legalEntityId;
            SignedByName = signedByName;
        }
    }
}
