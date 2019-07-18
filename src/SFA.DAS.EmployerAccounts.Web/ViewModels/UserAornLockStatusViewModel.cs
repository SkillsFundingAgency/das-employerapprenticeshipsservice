using System;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class UserAornLockStatusViewModel
    {
        public bool IsLocked { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int RemainingAttempts { get; set; }
    }
}
