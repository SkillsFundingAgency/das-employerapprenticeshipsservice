using System.Collections.Generic;
using SFA.DAS.NServiceBus;

namespace SFA.DAS.EmployerAccounts.Messages.Events
{
    public class UserRoleUpdatedEvent : Event
    {
        public long AccountId { get; }
        public string UserRef { get; }
        public HashSet<short> Roles { get; } = new HashSet<short>();

        public UserRoleUpdatedEvent(long accountId, string userRef, HashSet<short> roles)
        {
            AccountId = accountId;
            UserRef = userRef;
            Roles = roles;
        }
    }
}
