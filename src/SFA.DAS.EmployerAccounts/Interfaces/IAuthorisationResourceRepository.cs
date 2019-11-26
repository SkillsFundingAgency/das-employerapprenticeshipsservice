using SFA.DAS.EmployerAccounts.Models.Account;
using System.Collections.Generic;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface IAuthorisationResourceRepository
    {
        List<ResourceRoute> Get(); 
    }
}
