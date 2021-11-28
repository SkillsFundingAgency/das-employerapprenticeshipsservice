﻿using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface ICommitmentsV2ApiClient
    {
        Task<GetTransferRequestSummaryResponse> GetTransferRequests(long accountId);

        Task<GetApprenticeshipStatusSummaryResponse> GetEmployerAccountSummary(long employerAccountId);

        Task<GetCohortsResponse> GetCohorts(GetCohortsRequest request);

        Task<GetApprenticeshipResponse> GetApprenticeship(long apprenticeshipId);

        Task<GetDraftApprenticeshipsResponse> GetDraftApprenticeships(long cohortId);

        Task<GetApprenticeshipsResponse> GetApprenticeships(GetApprenticeshipsRequest request);
    }
}
