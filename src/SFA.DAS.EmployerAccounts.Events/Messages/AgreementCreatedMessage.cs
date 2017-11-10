using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("agreement_created")]
    public class AgreementCreatedMessage : Message
    {
        public AgreementCreatedMessage() : base(string.Empty, string.Empty)
        {

        }

        public AgreementCreatedMessage(string hashedAccountId, long aggreementId, string companyName, string signedByName) : base(signedByName, hashedAccountId)
        {
            AgreementId = aggreementId;
            CompanyName = companyName;
        }

        public string CompanyName { get; }
        public long AgreementId { get; }
    }
}
