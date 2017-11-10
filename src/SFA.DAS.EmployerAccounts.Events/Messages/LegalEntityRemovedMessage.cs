using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("legal_entity_removed")]
    public class LegalEntityRemovedMessage : Message
    {
        public LegalEntityRemovedMessage():base(string.Empty, string.Empty)
        {

        }

        public LegalEntityRemovedMessage(string hashedAccountId,  long aggreementId, bool agreementSigned, string signedByName) : base(signedByName, hashedAccountId)
        {
            AgreementId = aggreementId;
            AgreementSigned = agreementSigned;
        }

        public string CompanyName { get; set; }
        public long AgreementId { get; }
        public bool AgreementSigned { get; }
    }
}
