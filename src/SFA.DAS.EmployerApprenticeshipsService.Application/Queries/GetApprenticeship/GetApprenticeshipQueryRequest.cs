using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetApprenticeship
{
    public class GetApprenticeshipQueryRequest : IAsyncRequest<GetApprenticeshipQueryResponse>
    {
        public long AccountId { get; set; }

        public long ApprenticeshipId { get; set; }
    }
}