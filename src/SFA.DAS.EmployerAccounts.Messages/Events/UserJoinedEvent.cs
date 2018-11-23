using System;
using SFA.DAS.NServiceBus;

namespace SFA.DAS.EmployerAccounts.Messages.Events
{
    public class UserJoinedEvent : Event
    {
        public long AccountId { get; set; }
        public string UserName { get; set; }
        public Guid UserRef { get; set; }
    }
}
