using System.Collections.Generic;
using SFA.DAS.EmployerApprenticeshipsService.Domain;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetStandards
{
    public class GetStandardsQueryResponse
    {
        public List<Standard> Standards { get; set; }
    }
}