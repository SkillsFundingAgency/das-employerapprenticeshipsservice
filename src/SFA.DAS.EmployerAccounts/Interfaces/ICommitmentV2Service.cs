using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.EmployerAccounts.Models.Commitments;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface ICommitmentV2Service
    {
        Task<IEnumerable<CohortV2>> GetCohortsV2(long? accountId);        

        Task<IEnumerable<Apprenticeship>> GetDraftApprenticeships(long cohortId);

        Task<IEnumerable<Apprenticeship>> GetApprenticeships(long accountId);

    }

    public class CohortFilter
    {
        public int Take { get; set; }
        
    }
}


