using SFA.DAS.EmployerAccounts.Models.Commitments;
using System.Collections.Generic;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountCohort
{
    public class GetAccountCohortResponse
    {
        public IEnumerable<CohortV2> CohortV2 { get; set; }

        public string HashedCohortReference { get; set; }

        public string HashedDraftApprenticeshipId { get; set; }

    }
}
