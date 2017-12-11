using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("add_paye_scheme")]
    public class PayeSchemeCreatedMessage
    {
        public long AccountId { get; set; }
        public string CreatedByUserId { get; set; }
        public string EmpRef { get; set; }
    }
}