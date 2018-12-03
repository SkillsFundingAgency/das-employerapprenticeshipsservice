using System.ComponentModel.DataAnnotations;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Commands.DeleteSentTransferConnectionInvitation;
using SFA.DAS.EmployerAccounts.Dtos;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class RejectedTransferConnectionInvitationViewModel : MembershipMessage
    {
        [Required(ErrorMessage = "Option required")]
        [RegularExpression("Confirm|GoToTransfersPage", ErrorMessage = "Option required")]
        public string Choice { get; set; }

        [Required]
        public int? TransferConnectionInvitationId { get; set; }

        public TransferConnectionInvitationDto TransferConnectionInvitation { get; set; }
    }
}