using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Models.Recruit;

namespace SFA.DAS.EmployerAccounts.Features
{
    public interface IDasRecruitService
    {
        Task<VacanciesSummary> GetVacanciesByLegalEntity(string hashedAccountId, long legalEntityId,
            CancellationToken cancellationToken = default);
    }
}
