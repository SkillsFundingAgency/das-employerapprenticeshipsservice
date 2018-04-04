using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("levy_declarations_processed")]
    public class LevyDeclarationsProcessedEvent : IAccountEvent
    {
        public long AccountId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PayrollYear { get; set; }
        public short? PayrollMonth { get; set; }
        public decimal LevyDeclaredInMonth { get; set; }
    }
}