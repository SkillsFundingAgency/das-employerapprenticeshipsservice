using System;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    public interface IEvent
    {
        DateTime CreatedAt { get; }
    }
}