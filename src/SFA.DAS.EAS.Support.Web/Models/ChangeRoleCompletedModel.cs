namespace SFA.DAS.EAS.Support.Web.Models;

public class ChangeRoleCompletedModel
{
    public string ReturnToTeamUrl { get; set; }
    public bool Success { get; set; }
    public string MemberEmail { get; set; }
    public int Role { get; set; }
}