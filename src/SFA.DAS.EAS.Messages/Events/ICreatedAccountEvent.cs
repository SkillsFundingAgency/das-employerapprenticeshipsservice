using NServiceBus;
using System;

namespace SFA.DAS.EAS.Messages.Events
{
    public interface ICreatedAccountEvent : IEvent
    {
        long AccountId { get; set; }
        DateTime Created { get; set; }
        string UserName { get; set; }
        Guid UserRef { get; set; }
    }
}
