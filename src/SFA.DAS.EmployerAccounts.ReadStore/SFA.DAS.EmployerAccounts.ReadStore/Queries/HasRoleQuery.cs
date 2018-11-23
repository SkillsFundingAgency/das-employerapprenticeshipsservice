using System;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.ReadStore.Queries
{
    internal class HasRoleQuery : IReadStoreRequest<HasRoleQueryResult>
    {
        public Guid UserRef { get; set; }
        public long EmployerAccountId { get; set; }
        public UserRole[] UserRoles { get; set; }

        public HasRoleQuery(Guid userRef, long employerAccountId, UserRole[] userRoles)
        {
            UserRef = userRef;
            EmployerAccountId = employerAccountId;
            UserRoles = userRoles;
        }
    }
}
