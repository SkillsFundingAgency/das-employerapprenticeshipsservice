using System;
using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.Types.Models;
using SFA.DAS.NServiceBus;

namespace SFA.DAS.EmployerAccounts.Messages.Events
{
    public class AccountUserRolesUpdatedEvent : Event
    {
        public long AccountId { get; }
        public Guid UserRef { get; }
        public HashSet<UserRole> Roles { get; }

        public AccountUserRolesUpdatedEvent(long accountId, Guid userRef, HashSet<UserRole> roles, DateTime created)
        {
            AccountId = accountId;
            UserRef = userRef;
            Roles = roles;
            Created = created;
        }
    }
}
