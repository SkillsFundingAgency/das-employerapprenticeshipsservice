using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("agreement_signed")]
    public class AgreementSignedMessage : PersonMessage
    {
        public AgreementSignedMessage() :base(string.Empty)
        {

        }

        public AgreementSignedMessage(long accountId, long legalEntityId, long aggreementId, string providerName, string signedByName) :base(signedByName)
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
