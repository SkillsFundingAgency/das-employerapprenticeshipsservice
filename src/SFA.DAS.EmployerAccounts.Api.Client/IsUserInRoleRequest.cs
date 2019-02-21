using System;
using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.Api.Client
{
    public class IsUserInRoleRequest
    {
        public Guid UserRef { get; set; }
        public long AccountId { get; set; }
        public HashSet<UserRole> Roles { get; set; }
    }
}