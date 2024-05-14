namespace SFA.DAS.EAS.Account.Api.Requests;

public class ChangeTeamMemberRoleRequest
{
    public string HashedAccountId { get; set; }
    public string Email { get; set; }
    public int Role { get; set; }
    public string ExternalUserId { get; set; }
}