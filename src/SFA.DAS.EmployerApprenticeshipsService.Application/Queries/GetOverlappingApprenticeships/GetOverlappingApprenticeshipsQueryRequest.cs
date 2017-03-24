using System.Collections.Generic;

using MediatR;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.EAS.Application.Queries.GetOverlappingApprenticeships
{
    public class GetOverlappingApprenticeshipsQueryRequest : IAsyncRequest<GetOverlappingApprenticeshipsQueryResponse>
    {
        public IEnumerable<Apprenticeship> Apprenticeship { get; set; }
    }
}
