using SFA.DAS.EmployerAccounts.Models.Account;
using System.Collections.Generic;
using System.Security.Claims;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface IAuthorisationResourceRepository
    {
        IEnumerable<AuthorizationResource> Get(ClaimsIdentity claimsIdentity); 
    }
}
