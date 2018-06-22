using NServiceBus;
using System;

namespace SFA.DAS.EAS.Messages.Events
{
    public class ChangedAccountNameEvent : IEvent
    {
        public long AccountId { get; set; }
        public DateTime Created { get; set; }
        public string UserName { get; set; }
        public Guid UserRef { get; set; }
        public string PreviousName { get; set; }
        public string CurrentName { get; set; }
    }
}
