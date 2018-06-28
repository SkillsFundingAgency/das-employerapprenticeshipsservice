using System;
using SFA.DAS.NServiceBus;

namespace SFA.DAS.EAS.Messages.Events
{
    public class UserJoinedEvent : Event
    {
        public long AccountId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; }
        public Guid UserRef { get; set; }
    }
}
