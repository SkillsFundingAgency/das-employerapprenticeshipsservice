using System;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.Messages.Events
{
    public class AccountUserRolesUpdatedEvent
    {
        public long AccountId { get; }
        public Guid UserRef { get; }
        public UserRole Role { get; }
        public DateTime Created { get; set; }

        public AccountUserRolesUpdatedEvent(long accountId, Guid userRef, UserRole role, DateTime created)
        {
            AccountId = accountId;
            UserRef = userRef;
            Role = role;
            Created = created;
        }
    }
}
