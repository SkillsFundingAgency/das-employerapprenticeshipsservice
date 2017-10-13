using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetPriceHistoryQueryRequest
{
    public class GetPriceHistoryQueryRequest : IAsyncRequest<GetPriceHistoryQueryResponse>
    {
        public long ApprenticeshipId { get; set; }

        public long AccountId { get; set; }
    }
}