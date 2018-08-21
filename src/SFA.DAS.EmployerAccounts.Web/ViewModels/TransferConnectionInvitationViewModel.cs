using SFA.DAS.EmployerAccounts.Commands;
using SFA.DAS.EmployerAccounts.Dtos;
using System.ComponentModel.DataAnnotations;

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