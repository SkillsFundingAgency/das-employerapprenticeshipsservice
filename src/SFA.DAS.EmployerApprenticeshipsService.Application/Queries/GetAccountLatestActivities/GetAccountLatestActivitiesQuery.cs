using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetAccountLatestActivities
{
    public class GetAccountLatestActivitiesQuery : IAsyncRequest<GetAccountLatestActivitiesResponse>
    {
        public long AccountId { get; set; }
    }
}