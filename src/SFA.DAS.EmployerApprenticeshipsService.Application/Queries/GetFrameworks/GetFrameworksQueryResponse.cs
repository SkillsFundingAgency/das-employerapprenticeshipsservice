using System.Collections.Generic;
using SFA.DAS.EmployerApprenticeshipsService.Domain;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetFrameworks
{
    public class GetFrameworksQueryResponse
    {
        public List<Framework> Frameworks { get; set; }
    }
}