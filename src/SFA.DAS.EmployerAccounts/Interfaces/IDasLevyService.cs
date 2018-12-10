using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Models.Levy;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface IDasLevyService
    {
        Task<IEnumerable<DasEnglishFraction>> GetEnglishFractionHistory(long accountId, string empRef);
    }
}