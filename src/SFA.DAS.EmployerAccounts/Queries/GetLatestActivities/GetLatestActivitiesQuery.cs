using MediatR;
using SFA.DAS.EmployerAccounts.Messages;

namespace SFA.DAS.EmployerAccounts.Queries.GetLatestActivities
{
    public class GetLatestActivitiesQuery : MembershipMessage, IAsyncRequest<GetLatestActivitiesResponse>
    {
    }
}