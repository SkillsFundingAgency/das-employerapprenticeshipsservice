using System;
using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.ReadStore.Application.Queries
{
    internal class HasRoleQuery : IReadStoreRequest<bool>
    {
        public Guid UserRef { get; }
        public long EmployerAccountId { get; }
        public HashSet<UserRole> UserRoles { get; }

        public HasRoleQuery(Guid userRef, long employerAccountId, HashSet<UserRole> userRoles)
        {
            UserRef = userRef;
            EmployerAccountId = employerAccountId;
            UserRoles = userRoles;
        }
    }
}
