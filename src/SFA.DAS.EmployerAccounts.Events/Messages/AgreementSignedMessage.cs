using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("agreement_signed")]
    public class AgreementSignedMessage : Message
    {
        public AgreementSignedMessage() :base(string.Empty, String.Empty)
        {

        }

        public AgreementSignedMessage(string hashedAccountId, long legalEntityId, long aggreementId, string providerName, string signedByName) :base(signedByName, hashedAccountId)
        {
            LegalEntityId = legalEntityId;
            AgreementId = aggreementId;
            ProviderName = providerName;
        }

        public string ProviderName { get; }
        public long LegalEntityId { get; }
        public long AgreementId { get; }  
    }
}
