using Newtonsoft.Json;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.EmployerAccounts.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class CommitmentsV2Service : ICommitmentV2Service
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public CommitmentsV2Service(ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<GetApprenticeshipsResponse> GetApprenticeship(long? accountId)
        {
            var apprenticeship = await _commitmentsApiClient.GetApprenticeships
                (new SFA.DAS.CommitmentsV2.Api.Types.Requests.GetApprenticeshipsRequest { AccountId = accountId });
            return apprenticeship;
        }

        public async Task<GetCohortsResponse> GetCohorts(long? accountId)
        {
            var cohorts = await _commitmentsApiClient.GetCohorts(new CommitmentsV2.Api.Types.Requests.GetCohortsRequest { AccountId = accountId });
            if (cohorts == null )
            {
                return new GetCohortsResponse(new List<CohortSummary>());
            }
            return cohorts;
        }

        public async Task<GetDraftApprenticeshipsResponse> GetDraftApprenticeships(long cohortId)
        {
            var draftApprenticeshipsResponse = await _commitmentsApiClient.GetDraftApprenticeships(cohortId);
            if (draftApprenticeshipsResponse == null)
            {
                return new GetDraftApprenticeshipsResponse();
            }
            
            return draftApprenticeshipsResponse;
        }
    }
}
