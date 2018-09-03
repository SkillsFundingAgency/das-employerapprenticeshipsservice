using SFA.DAS.EmployerFinance.Commands.ApproveTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Commands.RejectTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Dtos;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EmployerFinance.Web.ViewModels
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