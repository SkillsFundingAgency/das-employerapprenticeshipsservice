using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Authorization
{
    public interface IAuthorizationHandler
    {
        Task<AuthorizationResult> CanAccessAsync(IAuthorizationContext authorizationContext, Feature feature);
    }
}
