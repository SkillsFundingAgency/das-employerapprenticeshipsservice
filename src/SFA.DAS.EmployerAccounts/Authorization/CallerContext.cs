using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Authorization
{
    public class CallerContext : ICallerContext
    {
        public string AccountHashedId { get; set; }
        public long? AccountId { get; set; }
        public Guid? UserRef { get; set; }
    }
}
