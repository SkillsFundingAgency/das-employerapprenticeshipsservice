using System.Threading.Tasks;
using SFA.DAS.EAS.Portal.Client.Services.DasRecruit.Models;

namespace SFA.DAS.EAS.Portal.Client.Services.DasRecruit
{
    public interface IDasRecruitService
    {
        Task<VacanciesSummary> GetVacanciesSummary(long accountId);
    }
}