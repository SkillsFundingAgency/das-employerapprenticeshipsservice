using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.EmployerAccounts.Models.Commitments;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface ICommitmentV2Service
    {
        Task<GetApprenticeshipsResponse> GetApprenticeship(long? accountId);

        Task<GetCohortsResponse> GetCohorts(long? accountId);

        Task<IEnumerable<CohortV2>> GetCohortsV2(long? accountId);

        Task<GetDraftApprenticeshipsResponse> GetDraftApprenticeships(long cohortId);

    }
}
