using System;
using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.ReadStore.Application.Queries
{
    internal class HasRoleQuery : IReadStoreRequest<bool>
    {
        public Guid UserRef { get; }
        public long AccountId { get; }
        public HashSet<UserRole> UserRoles { get; }

        public HasRoleQuery(Guid userRef, long accountId, HashSet<UserRole> userRoles)
        {
            UserRef = userRef;
            AccountId = accountId;
            UserRoles = userRoles;
        }
    }
}
