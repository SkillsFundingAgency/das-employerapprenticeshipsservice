using SFA.DAS.EmployerAccounts.Models.CommitmentsV2;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface ICommitmentV2Service
    {
        Task<IEnumerable<Cohort>> GetCohorts(long? accountId);        

        Task<IEnumerable<Apprenticeship>> GetDraftApprenticeships(Cohort cohort);

        Task<IEnumerable<Apprenticeship>> GetApprenticeships(long accountId);
    }
}


