using SFA.DAS.Authorization;

namespace SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitations
{
    public class TransferConnectionInvitationAuthorizationViewModel
    {
        public AuthorizationResult AuthorizationResult { get; set; }
        public bool IsValidSender { get; set; }
    }
}