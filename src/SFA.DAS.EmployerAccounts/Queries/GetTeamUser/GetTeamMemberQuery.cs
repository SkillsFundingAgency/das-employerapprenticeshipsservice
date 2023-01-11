namespace SFA.DAS.EmployerAccounts.Queries.GetTeamUser;

public class GetTeamMemberQuery : IAsyncRequest<GetTeamMemberResponse>
{
    public string HashedAccountId { get; set; }
    public string TeamMemberId { get; set; }
}