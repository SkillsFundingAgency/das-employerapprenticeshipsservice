using System.Collections.Generic;
using System.Linq;

using SFA.DAS.Commitments.Api.Types.Validation;

namespace SFA.DAS.EAS.Application.Queries.GetOverlappingApprenticeships
{
    public class GetOverlappingApprenticeshipsQueryResponse
    {
        public IEnumerable<ApprenticeshipOverlapValidationResult> Overlaps { get; set; }

        public IEnumerable<OverlappingApprenticeship> GetOverlappingApprenticeships(string uln)
        {
            return Overlaps.FirstOrDefault(m => m.Self.Uln == uln)
                       ?.OverlappingApprenticeships
                   ?? Enumerable.Empty<OverlappingApprenticeship>();
        }

        public IEnumerable<OverlappingApprenticeship> GetFirstOverlappingApprenticeships()
        {
            return Overlaps.FirstOrDefault()?.OverlappingApprenticeships
                   ?? Enumerable.Empty<OverlappingApprenticeship>();
        }
    }
}