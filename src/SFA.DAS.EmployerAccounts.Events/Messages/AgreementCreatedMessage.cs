using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("agreement_created")]
    public class AgreementCreatedMessage : Message
    {
        public AgreementCreatedMessage() : base(string.Empty, String.Empty)
        {

        }

        public AgreementCreatedMessage(string hashedAccountId, long legalEntityId, long aggreementId, string companyName, string signedByName) : base(signedByName, hashedAccountId)
        {
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
