using System;
using SFA.DAS.NServiceBus;

namespace SFA.DAS.EAS.Messages.Events
{
    public class CreatedAccountEvent : Event
    {
        public long AccountId { get; set; }
        public DateTime Created { get; set; }
        public string UserName { get; set; }
        public Guid UserRef { get; set; }
    }
}
