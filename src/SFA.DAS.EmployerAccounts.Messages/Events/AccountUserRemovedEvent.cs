using System;

namespace SFA.DAS.EmployerAccounts.Messages.Events
{
    public class AccountUserRemovedEvent
    {
        public long AccountId { get; }
        public Guid UserRef { get; }

        public DateTime Created { get; set; }

        public AccountUserRemovedEvent(long accountId, Guid userRef, DateTime created)
        {
            AccountId = accountId;
            UserRef = userRef;
            Created = created;
        }
    }
}
