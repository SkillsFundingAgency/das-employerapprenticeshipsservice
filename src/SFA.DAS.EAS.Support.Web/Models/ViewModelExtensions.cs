using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Support.Web.Models;

public static class ViewModelExtensions
{
    public static string GetTeamMemberStatus(this InvitationStatus status)
    {
        switch (status)
        {
            case InvitationStatus.Accepted:
                return "Active";
            case InvitationStatus.Pending:
                return "Invitation awaiting response";
            case InvitationStatus.Expired:
                return "Invitation expired";
            case InvitationStatus.Deleted:
                return string.Empty;
            default:
                return string.Empty;
        }
    }
}