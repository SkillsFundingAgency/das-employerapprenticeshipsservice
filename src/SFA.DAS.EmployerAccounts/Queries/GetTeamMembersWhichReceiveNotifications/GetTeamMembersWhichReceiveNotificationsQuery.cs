namespace SFA.DAS.EmployerAccounts.Queries.GetTeamMembersWhichReceiveNotifications;

public class GetTeamMembersWhichReceiveNotificationsQuery : IRequest<GetTeamMembersWhichReceiveNotificationsQueryResponse>
{
    public long AccountId { get; set; }
}