using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("agreement_signed")]
    public class AgreementSignedMessage : Message
    {
        public AgreementSignedMessage()
        {

        }

        public AgreementSignedMessage(long accountId, long aggreementId, string providerName, string signedByName, long legalEntityId) :base(signedByName, accountId)
        {
            AgreementId = aggreementId;
            ProviderName = providerName;
            LegalEntityId = legalEntityId;
        }

        public string ProviderName { get; }
        public long AgreementId { get; }

        public long LegalEntityId { get; }
    }
}
