using System.ComponentModel;

namespace SFA.DAS.EAS.Account.Api.Types
{
    public enum InvitationStatus : byte
    {
        [Description("Invitation awaiting response")]
        Pending = 1,
        [Description("Active")]
        Accepted = 2,
        [Description("Invitation expired")]
        Expired = 3,
        [Description("Invitation cancelled")]
        Deleted = 4
    }
}
