using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("agreement_signed")]
    public class AgreementSignedMessage : Message
    {
        public AgreementSignedMessage()
        {

        }

        public AgreementSignedMessage(long accountId, long legalEntityId, long aggreementId)
        {
            AccountId = accountId;
            LegalEntityId = legalEntityId;
            AgreementId = aggreementId;
        }

        public string ProviderName { get; } = "todo";

        public long AccountId { get; }
        public long LegalEntityId { get; }
        public long AgreementId { get; }
    }
}
