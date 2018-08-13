using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Authorization
{
    public class AccountContext : IAccountContext
    {
        public long Id { get; set; }
        public string HashedId { get; set; }
        public string PublicHashedId { get; set; }
    }
}
