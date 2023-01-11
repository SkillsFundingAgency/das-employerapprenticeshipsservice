namespace SFA.DAS.EmployerAccounts.Queries.GetTeamMembers;

public class GetTeamMembersRequest : IAsyncRequest<GetTeamMembersResponse>
{
    public string HashedAccountId { get; set; }
}