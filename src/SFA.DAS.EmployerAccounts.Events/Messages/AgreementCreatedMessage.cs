using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("agreement_created")]
    public class AgreementCreatedMessage : PersonMessage
    {
        public AgreementCreatedMessage() : base(string.Empty)
        {

        }

        public AgreementCreatedMessage(long accountId, long legalEntityId, long aggreementId, string companyName, string signedByName) : base(signedByName)
        {
            AccountId = accountId;
            LegalEntityId = legalEntityId;
            AgreementId = aggreementId;
            CompanyName = companyName;
        }

        public string CompanyName { get; }

        public long AccountId { get; }
        public long LegalEntityId { get; }
        public long AgreementId { get; }
    }
}
