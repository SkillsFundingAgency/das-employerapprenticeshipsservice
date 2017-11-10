using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("agreement_created")]
    public class AgreementCreatedMessage : Message
    {
        public AgreementCreatedMessage()
        {

        }

        public AgreementCreatedMessage(long accountId, long aggreementId, string companyName, string signedByName, long legalEntityId) : base(signedByName, accountId)
        {
            AgreementId = aggreementId;
            CompanyName = companyName;
            LegalEntityId = legalEntityId;
        }

        public string CompanyName { get; }
        public long AgreementId { get; }
        public long LegalEntityId { get; }
    }
}
