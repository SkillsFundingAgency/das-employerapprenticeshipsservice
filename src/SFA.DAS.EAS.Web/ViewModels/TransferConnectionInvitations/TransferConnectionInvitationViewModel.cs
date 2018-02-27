using SFA.DAS.EAS.Application.Dtos;

namespace SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitations
{
    public class TransferConnectionInvitationViewModel
    {
        public long AccountId { get; set; }
        public TransferConnectionInvitationDto TransferConnectionInvitation { get; set; }
    }
}