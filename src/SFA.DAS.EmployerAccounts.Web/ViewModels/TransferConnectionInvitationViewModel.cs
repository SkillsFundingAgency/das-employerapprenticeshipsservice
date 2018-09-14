using System.ComponentModel.DataAnnotations;
using SFA.DAS.EmployerAccounts.Commands.DeleteSentTransferConnectionInvitation;
using SFA.DAS.EmployerAccounts.Dtos;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class TransferConnectionInvitationViewModel
    {
        public long AccountId { get; set; }

        [Required(ErrorMessage = "Option required")]
        [RegularExpression("Confirm|GoToTransfersPage", ErrorMessage = "Option required")]
        public string Choice { get; set; }

        [Required]
        public DeleteTransferConnectionInvitationCommand DeleteTransferConnectionInvitationCommand { get; set; }

        public TransferConnectionInvitationDto TransferConnectionInvitation { get; set; }
    }
}