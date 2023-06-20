namespace SFA.DAS.EmployerAccounts.Queries.GetAccountTeamMembers;

public class GetAccountTeamMembersQuery : IRequest<GetAccountTeamMembersResponse>
{
    public string HashedAccountId { get; set; }
    public string ExternalUserId { get; set; }
}