using System;

namespace SFA.DAS.EmployerAccounts.Jobs.Data
{
    public class MembershipUser
    {
        public long UserId { get; set; }
        public Guid UserRef { get; set; }
        public long AccountId { get; set; }
        public short Role { get; set; }
    }
}
