using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Authorization
{
    public class UserContext : IUserContext
    {
        public long Id { get; set; }
        public Guid Ref { get; set; }
        public string Email { get; set; }
    }
}
