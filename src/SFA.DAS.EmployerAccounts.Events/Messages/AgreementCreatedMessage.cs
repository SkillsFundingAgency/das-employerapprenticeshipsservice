using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("agreement_created")]
    public class AgreementCreatedMessage : Message
    {
        public string CompanyName { get; }
        public long AgreementId { get; }
        public long LegalEntityId { get; }
        public string CreatedByName { get; }

        public AgreementCreatedMessage(long accountId, long aggreementId, string companyName, string createdByName, long legalEntityId) : base(accountId)
        {
            AgreementId = aggreementId;
            CompanyName = companyName;
            LegalEntityId = legalEntityId;
            CreatedByName = createdByName;
        }
    }
}
