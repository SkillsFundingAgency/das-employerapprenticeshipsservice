using System;

namespace SFA.DAS.EmployerAccounts.Models.UserProfile
{
    public class UserAornPayeStatus
    {
        public bool IsLocked { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
