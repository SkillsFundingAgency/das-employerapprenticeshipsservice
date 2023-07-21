namespace SFA.DAS.EmployerAccounts.Queries.GetUserAccountRole;

public class GetUserAccountRoleQuery : IRequest<GetUserAccountRoleResponse>
{
    public long AccountId { get; set; }
    public string ExternalUserId { get; set; }
}