using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EAS.Application.Queries.GetLatestActivities
{
    public class GetLatestActivitiesQuery : MembershipMessage, IAsyncRequest<GetLatestActivitiesResponse>
    {
    }
}