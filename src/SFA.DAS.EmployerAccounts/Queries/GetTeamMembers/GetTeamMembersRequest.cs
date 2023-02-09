namespace SFA.DAS.EmployerAccounts.Queries.GetTeamMembers;

public class GetTeamMembersRequest : IRequest<GetTeamMembersResponse>
{
    public long AccountId { get; }

	public GetTeamMembersRequest(long accountId)
	{
		AccountId = accountId;
	}
}