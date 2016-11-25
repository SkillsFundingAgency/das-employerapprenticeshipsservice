using System.ComponentModel;

namespace SFA.DAS.EAS.Web.Models
{
    public enum RequestStatus
    {
        [Description("New request")]
        NewRequest,

        [Description("Sent to provider")]
        SentToProvider,

        [Description("Ready for review")]
        ReadyForReview,

        [Description("With Provider for approval")]
        WithProviderForApproval,

        [Description("Ready for approval")]
        ReadyForApproval,

        [Description("Approved")]
        Approved
    }
}