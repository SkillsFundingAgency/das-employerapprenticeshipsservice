using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Configuration
{
    public class UserAornPayeLockConfiguration
    {
        public int NumberOfPermittedAttempts { get; set; }
        public int PermittedAttemptsTimeSpanMinutes { get; set; }
        public int LockoutTimeSpanMinutes { get; set; }
    }
}
