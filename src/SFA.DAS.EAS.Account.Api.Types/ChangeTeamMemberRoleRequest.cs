namespace SFA.DAS.EAS.Account.Api.Types;

public record ChangeTeamMemberRoleRequest
{
    public ChangeTeamMemberRoleRequest(string HashedAccountId, string Email, int Role, string ExternalUserId) { }
}