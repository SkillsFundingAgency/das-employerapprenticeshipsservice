using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Authorization
{
    public interface IMembershipContext
    {
        Role Role { get; }
    }
}
