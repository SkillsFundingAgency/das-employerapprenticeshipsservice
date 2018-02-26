using MediatR;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Application.Queries.GetLatestActivities
{
    public class GetLatestActivitiesQuery : AuthorizedMessage, IAsyncRequest<GetLatestActivitiesResponse>
    {
    }
}