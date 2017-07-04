using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface IEnglishFractionRepository
    {
        Task<DateTime> GetLastUpdateDate();
        Task<IEnumerable<DasEnglishFraction>> GetAllEmployerFractions(string employerReference);
        Task CreateEmployerFraction(DasEnglishFraction fractions, string employerReference);
        Task SetLastUpdateDate(DateTime dateUpdated);
        Task<DasEnglishFraction> GetCurrentFractionForScheme(long accountId, string employerReference);
    }
}
