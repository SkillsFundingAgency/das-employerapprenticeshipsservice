using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.DasRecruitService.Models;

namespace SFA.DAS.EAS.DasRecruitService.Services
{
    public interface IDasRecruitService
    {
        Task<VacanciesSummary> GetVacanciesSummary(long accountId);
    }
}