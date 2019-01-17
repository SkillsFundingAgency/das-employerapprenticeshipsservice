using System;

namespace SFA.DAS.EmployerAccounts.Api.Client
{
    public class IsUserInAnyRoleRequest
    {
        public Guid UserRef { get; set; }
        public long AccountId { get; set; }
    }
}