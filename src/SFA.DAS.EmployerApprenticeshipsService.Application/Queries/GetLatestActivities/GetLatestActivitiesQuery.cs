using MediatR;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Application.Queries.GetLatestActivities
{
    public class GetLatestActivitiesQuery : MembershipMessage, IAsyncRequest<GetLatestActivitiesResponse>
    {
    }
}