using System;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.Messages.Events
{
    public class UserJoinedEvent
    {
        public long AccountId { get; set; }
        public string HashedAccountId { get; set; }
        public string UserName { get; set; }
        public Guid UserRef { get; set; }
        public UserRole Role { get; set; }
        public DateTime Created { get; set; }
    }
}
