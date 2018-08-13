using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Authorization
{
    public class AuthorizationContext : IAuthorizationContext
    {
        public IAccountContext AccountContext { get; set; }
        public IMembershipContext MembershipContext { get; set; }
        public IUserContext UserContext { get; set; }
    }
}
