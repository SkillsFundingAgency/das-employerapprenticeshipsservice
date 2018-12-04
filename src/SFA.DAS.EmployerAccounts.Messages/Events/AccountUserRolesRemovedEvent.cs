using System;
using SFA.DAS.NServiceBus;

namespace SFA.DAS.EmployerAccounts.Messages.Events
{
    public class AccountUserRolesRemovedEvent : Event
    {
        public long AccountId { get; }
        public Guid UserRef { get; }
        public AccountUserRolesRemovedEvent(long accountId, Guid userRef, DateTime created)
        {
            AccountId = accountId;
            UserRef = userRef;
            Created = created;
        }
    }
}
