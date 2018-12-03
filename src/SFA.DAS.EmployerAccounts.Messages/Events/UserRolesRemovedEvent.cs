using System;
using SFA.DAS.NServiceBus;

namespace SFA.DAS.EmployerAccounts.Messages.Events
{
    public class UserRolesRemovedEvent : Event
    {
        public long AccountId { get; }
        public long UserId { get; }
        public UserRolesRemovedEvent(long accountId, long userId, DateTime created)
        {
            AccountId = accountId;
            UserId = userId;
            Created = created;
        }
    }
}
