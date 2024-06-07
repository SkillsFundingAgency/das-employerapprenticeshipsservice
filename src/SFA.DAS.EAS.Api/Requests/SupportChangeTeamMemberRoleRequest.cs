namespace SFA.DAS.EAS.Account.Api.Requests;

public class SupportChangeTeamMemberRoleRequest
{
    public string HashedAccountId { get; set; }
    public string Email { get; set; }
    public int Role { get; set; }
}