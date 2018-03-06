namespace SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitations
{
    public class TransferConnectionRolesViewModel 
    {
        public bool IsPendingReceiver { get; set; }
        public bool IsApprovedReceiver { get; set; }
        public bool IsReceiver => IsPendingReceiver || IsApprovedReceiver;
    }
}