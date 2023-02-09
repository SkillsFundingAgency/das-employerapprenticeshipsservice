namespace SFA.DAS.EmployerAccounts.Queries.GetTeamUser;

public class GetTeamMemberQuery : IRequest<GetTeamMemberResponse>
{
    public long AccountId { get; set; }
    public string TeamMemberId { get; set; }
}