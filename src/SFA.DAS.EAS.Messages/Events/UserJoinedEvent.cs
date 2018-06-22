using NServiceBus;
using System;

namespace SFA.DAS.EAS.Messages.Events
{
    public class UserJoinedEvent : IEvent
    {
        public long AccountId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; }
        public Guid UserRef { get; set; }
    }
}
