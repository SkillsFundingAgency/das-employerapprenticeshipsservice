using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Authorization
{
    public interface IAuthorizationContext
    {
        IAccountContext AccountContext { get; }
        IMembershipContext MembershipContext { get; }
        IUserContext UserContext { get; }
    }
}
