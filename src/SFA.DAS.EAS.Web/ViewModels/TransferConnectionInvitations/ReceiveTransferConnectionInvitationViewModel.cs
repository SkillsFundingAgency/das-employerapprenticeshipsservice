using System.ComponentModel.DataAnnotations;
using SFA.DAS.EAS.Application.Commands.ApproveTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Commands.RejectTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Dtos;

namespace SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitations
{
    public class ReceiveTransferConnectionInvitationViewModel
    {
        [Required]
        public ApproveTransferConnectionInvitationCommand ApproveTransferConnectionInvitationCommand { get; set; }

        [Required]
        public RejectTransferConnectionInvitationCommand RejectTransferConnectionInvitationCommand { get; set; }

        [Required(ErrorMessage = "Option required")]
        [RegularExpression("Approve|Reject", ErrorMessage = "Option required")]
        public string Choice { get; set; }

        public TransferConnectionInvitationDto TransferConnectionInvitation { get; set; }
    }
}