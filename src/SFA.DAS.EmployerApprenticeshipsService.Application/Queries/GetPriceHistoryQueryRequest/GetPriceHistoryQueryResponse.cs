using System.Collections.Generic;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.EAS.Application.Queries.GetPriceHistoryQueryRequest
{
    public class GetPriceHistoryQueryResponse
    {
        public List<PriceHistory> History { get; set; }
    }
}