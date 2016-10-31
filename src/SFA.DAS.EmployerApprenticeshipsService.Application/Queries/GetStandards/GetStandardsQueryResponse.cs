using System.Collections.Generic;
using SFA.DAS.EAS.Domain;

namespace SFA.DAS.EAS.Application.Queries.GetStandards
{
    public class GetStandardsQueryResponse
    {
        public List<Standard> Standards { get; set; }
    }
}