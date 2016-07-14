using System.Collections.Generic;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain
{
    public class AggregationData
    {
        public int AccountId { get; set; }

        public List<AggregationLine> Data { get; set; }
    }
}