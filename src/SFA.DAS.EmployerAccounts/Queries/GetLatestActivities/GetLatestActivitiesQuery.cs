using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerAccounts.Queries.GetLatestActivities
{
    public class GetLatestActivitiesQuery : MembershipMessage, IAsyncRequest<GetLatestActivitiesResponse>
    {
    }
}