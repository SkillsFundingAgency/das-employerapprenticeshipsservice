using System.Collections.Generic;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain
{
    public class AggregationData
    {
        public long AccountId { get; set; }

        public List<AggregationLine> Data { get; set; }
    }
}