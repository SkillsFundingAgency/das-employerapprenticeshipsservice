using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;

namespace SFA.DAS.EmployerAccounts.Interfaces;

public interface ICommitmentsV2ApiClient
{
    Task<GetApprenticeshipStatusSummaryResponse> GetEmployerAccountSummary(long accountId);

    Task<GetCohortsResponse> GetCohorts(GetCohortsRequest request);

    Task<GetApprenticeshipResponse> GetApprenticeship(long apprenticeshipId);

    Task<GetDraftApprenticeshipsResponse> GetDraftApprenticeships(long cohortId);

    Task<GetApprenticeshipsResponse> GetApprenticeships(GetApprenticeshipsRequest request);
}