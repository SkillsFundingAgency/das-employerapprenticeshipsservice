using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [QueueName("agreement_created_notifications")]
    public class AgreementCreatedMessage
    {
        public long AccountId { get; set; }
        public long LegalEntityId { get; set; }
        public long AgreementId { get; set; }
    }
}
