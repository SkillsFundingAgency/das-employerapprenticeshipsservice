using System;
using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.Types.Models;
using SFA.DAS.NServiceBus;

namespace SFA.DAS.EmployerAccounts.Messages.Events
{
    public class UserRolesUpdatedEvent : Event
    {
        public long AccountId { get; }
        public Guid UserRef { get; }
        public long UserId { get; }
        public HashSet<UserRole> Roles { get; }
        public UserRolesUpdatedEvent(long accountId, Guid userRef, long userId, HashSet<UserRole> roles, DateTime created)
        {
            AccountId = accountId;
            UserRef = userRef;
            UserId = userId;
            Roles = roles;
            Created = created;
        }
    }
}
