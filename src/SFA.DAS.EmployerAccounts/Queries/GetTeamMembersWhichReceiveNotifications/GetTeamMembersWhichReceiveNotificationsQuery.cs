namespace SFA.DAS.EmployerAccounts.Queries.GetTeamMembersWhichReceiveNotifications;

public class GetTeamMembersWhichReceiveNotificationsQuery : IAsyncRequest<GetTeamMembersWhichReceiveNotificationsQueryResponse>
{
    public string HashedAccountId { get; set; }
}