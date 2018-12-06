using System;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.Jobs.Data
{
    internal class MembershipUser
    {
        public long UserId { get; set; }
        public Guid UserRef { get; set; }
        public long AccountId { get; set; }
        public short Role { get; set; }
    }
}
