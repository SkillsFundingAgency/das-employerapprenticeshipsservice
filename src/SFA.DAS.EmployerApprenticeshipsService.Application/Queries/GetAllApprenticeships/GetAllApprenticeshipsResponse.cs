using System.Collections.Generic;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.EAS.Application.Queries.GetAllApprenticeships
{
    public class GetAllApprenticeshipsResponse
    {
        public List<Apprenticeship> Apprenticeships { get; set; }
    }
}