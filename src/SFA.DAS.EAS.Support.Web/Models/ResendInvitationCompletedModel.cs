namespace SFA.DAS.EAS.Support.Web.Models;

public class ResendInvitationCompletedModel
{
    public string ReturnToTeamUrl { get; set; }
    public bool Success { get; set; }
    public string MemberEmail { get; set; }
}