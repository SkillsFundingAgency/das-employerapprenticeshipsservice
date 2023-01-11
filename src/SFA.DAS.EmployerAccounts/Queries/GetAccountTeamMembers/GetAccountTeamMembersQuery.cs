namespace SFA.DAS.EmployerAccounts.Queries.GetAccountTeamMembers;

public class GetAccountTeamMembersQuery : IAsyncRequest<GetAccountTeamMembersResponse>
{
    public string HashedAccountId { get; set; }
    public string ExternalUserId { get; set; }
}