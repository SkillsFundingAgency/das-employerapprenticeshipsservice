using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Portal.Client.Types;

namespace SFA.DAS.EAS.Portal.Client.Services.DasRecruit
{
    internal interface IDasRecruitService
    {
        Task<IEnumerable<Vacancy>> GetVacancies(long accountId, int maxVacanciesToGet = int.MaxValue);
    }
}