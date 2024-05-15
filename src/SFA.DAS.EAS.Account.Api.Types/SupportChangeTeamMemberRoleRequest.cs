namespace SFA.DAS.EAS.Account.Api.Types;

public record SupportChangeTeamMemberRoleRequest
{
    public string HashedAccountId { get; set; }
    public string Email { get; set; }
    public int Role { get; set; }
}