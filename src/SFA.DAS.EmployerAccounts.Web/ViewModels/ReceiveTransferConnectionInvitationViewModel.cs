using SFA.DAS.EmployerAccounts.Commands;
using SFA.DAS.EmployerAccounts.DTOs;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
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