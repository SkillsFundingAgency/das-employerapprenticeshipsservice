using System.ComponentModel.DataAnnotations;
using SFA.DAS.EAS.Application.Commands.DeleteSentTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Dtos;

namespace SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitations
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