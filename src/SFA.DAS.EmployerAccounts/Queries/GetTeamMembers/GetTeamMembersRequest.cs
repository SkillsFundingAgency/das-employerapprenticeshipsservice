namespace SFA.DAS.EmployerAccounts.Queries.GetTeamMembers;

public class GetTeamMembersRequest : IRequest<GetTeamMembersResponse>
{
    public string HashedAccountId { get; set; }
}