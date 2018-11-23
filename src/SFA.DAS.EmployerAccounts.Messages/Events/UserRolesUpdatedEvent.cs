using System;
using System.Collections.Generic;
using SFA.DAS.NServiceBus;

namespace SFA.DAS.EmployerAccounts.Messages.Events
{
    public class UserRolesUpdatedEvent : Event
    {
        public long AccountId { get; }
        public string UserRef { get; }
        public HashSet<short> Roles { get; }
        public DateTime Updated { get; }

        public UserRolesUpdatedEvent(long accountId, string userRef, HashSet<short> roles, DateTime updated)
        {
            AccountId = accountId;
            UserRef = userRef;
            Roles = roles;
            Updated = updated;
        }
    }
}
