using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using System.Linq;

namespace SFA.DAS.EmployerAccounts.Queries.GetCohorts
{
    public class GetCohortsResponse
    {
        public SFA.DAS.CommitmentsV2.Api.Types.Responses.GetCohortsResponse CohortsResponse { get; set;  }

        public string HashedCohortReference { get; set; }

        public CohortSummary SingleCohort { get; set; }
    }
}
