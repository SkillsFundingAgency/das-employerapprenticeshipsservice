using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("legal_entity_removed")]
    public class LegalEntityRemovedMessage : Message
    {
        public long AgreementId { get; }
        public bool AgreementSigned { get; }
        public long LegalEntityId { get; }
        public string RemovedByName { get; }

        public LegalEntityRemovedMessage(long accountId,  long aggreementId, bool agreementSigned, string removedByName, long legalEntityId) : base(accountId)
        {
            RemovedByName = removedByName;
            AgreementId = aggreementId;
            AgreementSigned = agreementSigned;
            LegalEntityId = legalEntityId;
        }
    }
}
