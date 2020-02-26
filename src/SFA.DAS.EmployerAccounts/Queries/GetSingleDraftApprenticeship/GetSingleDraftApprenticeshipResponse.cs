using SFA.DAS.CommitmentsV2.Api.Types.Responses;

namespace SFA.DAS.EmployerAccounts.Queries.GetSingleDraftApprenticeship
{
    public class GetSingleDraftApprenticeshipResponse
    {
        public GetDraftApprenticeshipsResponse DraftApprenticeshipsResponse { get; set; }

        public string HashedDraftApprenticeshipId { get; set; }
    }
}
