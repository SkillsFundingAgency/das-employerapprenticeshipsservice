using System.ComponentModel;

namespace SFA.DAS.EAS.Web.Models
{
    public enum RequestStatus
    {
        None, // No use here.

        [Description("New request")]
        NewRequest,

        [Description("Sent to provider")]
        SentToProvider,

        [Description("Sent for review")]
        SentForReview,

        [Description("Ready for review")]
        ReadyForReview,

        [Description("Ready for approval")]
        ReadyForApproval,

        [Description("Approved")]
        Approved
    }
}