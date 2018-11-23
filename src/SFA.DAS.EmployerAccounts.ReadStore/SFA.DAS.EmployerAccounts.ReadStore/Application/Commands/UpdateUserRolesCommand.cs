using System;
using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.ReadStore.Application.Commands
{
    internal class UpdateUserRolesCommand : IReadStoreRequest<Unit>
    {
        public long AccountId { get; }
        public Guid UserRef { get; }
        public HashSet<UserRole> Roles { get; }
        public string MessageId { get; }
        public DateTime Updated { get; }

        public UpdateUserRolesCommand(long accountId, Guid userRef, HashSet<UserRole> roles, string messageId, DateTime updated)
        {
            AccountId = accountId;
            UserRef = userRef;
            Roles = roles;
            MessageId = messageId;
            Updated = updated;
        }
    }
}
