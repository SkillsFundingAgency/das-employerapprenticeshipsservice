using System.Collections.Generic;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.EAS.Application.Queries.ApprenticeshipSearch
{
    public class ApprenticeshipSearchQueryResponse
    {
        public List<Apprenticeship> Apprenticeships { get; set; }
        public Facets Facets { get; set; }
        public int TotalApprenticeships { get; set; }
    }
}
