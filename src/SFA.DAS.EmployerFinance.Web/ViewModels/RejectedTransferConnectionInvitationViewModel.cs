using SFA.DAS.EmployerFinance.Commands.DeleteSentTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Dtos;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EmployerFinance.Web.ViewModels
{
    public class RejectedTransferConnectionInvitationViewModel
    {
        [Required(ErrorMessage = "Option required")]
        [RegularExpression("Confirm|GoToTransfersPage", ErrorMessage = "Option required")]
        public string Choice { get; set; }

        [Required]
        public DeleteTransferConnectionInvitationCommand DeleteTransferConnectionInvitationCommand { get; set; }

        public TransferConnectionInvitationDto TransferConnectionInvitation { get; set; }
    }
}