using SFA.DAS.EAS.Application.Queries.GetAccountTransferRole;

namespace SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitations
{
    public class IndexTransferConnectionInvitationViewModel 
    {
        public bool IsPendingReceiver { get; set; }
        public bool IsApprovedReceiver { get; set; }
        public bool IsReceiver => IsPendingReceiver || IsApprovedReceiver;
    }
}