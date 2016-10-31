using System.Collections.Generic;
using SFA.DAS.EAS.Domain;

namespace SFA.DAS.EAS.Application.Queries.GetFrameworks
{
    public class GetFrameworksQueryResponse
    {
        public List<Framework> Frameworks { get; set; }
    }
}