using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.ReadStore.Queries
{
    public class HasRoleQuery : IApiRequest<HasRoleQueryResult>
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
