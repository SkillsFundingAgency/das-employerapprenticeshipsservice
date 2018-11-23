using System;
using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;

namespace SFA.DAS.EmployerAccounts.ReadStore.Application.Commands
{
    internal class UpdateUserRolesCommand : IReadStoreRequest<Unit>
    {
        public long AccountId { get; }
        public Guid UserRef { get; }
        public HashSet<short> Roles { get; }
        public string MessageId { get; }
        public DateTime Updated { get; }

        public UpdateUserRolesCommand(long accountId, Guid userRef, HashSet<short> roles, string messageId, DateTime updated)
        {
            AccountId = accountId;
            UserRef = userRef;
            Roles = roles;
            MessageId = messageId;
            Updated = updated;
        }
    }
}
