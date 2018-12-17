using System;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;

namespace SFA.DAS.EmployerAccounts.ReadStore.Application.Queries
{
    internal class IsUserInAnyRoleQuery : IReadStoreRequest<bool>
    {
        public Guid UserRef { get; }
        public long AccountId { get; }

        public IsUserInAnyRoleQuery(Guid userRef, long accountId)
        {
            UserRef = userRef;
            AccountId = accountId;
        }
    }
}
