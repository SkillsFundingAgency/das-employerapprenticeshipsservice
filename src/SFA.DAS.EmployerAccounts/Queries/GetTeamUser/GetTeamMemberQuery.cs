namespace SFA.DAS.EmployerAccounts.Queries.GetTeamUser;

public class GetTeamMemberQuery : IRequest<GetTeamMemberResponse>
{
    public string HashedAccountId { get; set; }
    public string TeamMemberId { get; set; }
}