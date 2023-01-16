namespace SFA.DAS.EmployerAccounts.Queries.GetTeamMembersWhichReceiveNotifications;

public class GetTeamMembersWhichReceiveNotificationsQuery : IRequest<GetTeamMembersWhichReceiveNotificationsQueryResponse>
{
    public string HashedAccountId { get; set; }
}