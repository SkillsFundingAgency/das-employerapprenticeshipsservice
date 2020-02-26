using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using System.Linq;

namespace SFA.DAS.EmployerAccounts.Queries.GetApprenticeship
{
    public class GetApprenticeshipResponse
    {
        public GetApprenticeshipsResponse ApprenticeshipDetailsResponse { get; set; }

        public int? ApprenticeshipsCount => ApprenticeshipDetailsResponse?.Apprenticeships?.Count();
    }
}
