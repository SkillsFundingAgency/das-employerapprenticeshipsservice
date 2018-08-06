using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface IAgreementService
    {
        Task<int?> GetAgreementVersionAsync(long accountId);
        Task RemoveFromCacheAsync(long accountId);
    }
}
