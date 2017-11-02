using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("agreement_signed")]
    public class AgreementSignedMessage : Message
    {
        public AgreementSignedMessage()
        {

        }

        public AgreementSignedMessage(long accountId, long legalEntityId, long aggreementId, string providerName)
        {
            LegalEntityId = legalEntityId;
            AgreementId = aggreementId;
            ProviderName = providerName;
            AccountId = accountId;
        }

        public string ProviderName { get; }

        public long LegalEntityId { get; }
        public long AgreementId { get; }
    
        public long AccountId { get; }
    }
}
