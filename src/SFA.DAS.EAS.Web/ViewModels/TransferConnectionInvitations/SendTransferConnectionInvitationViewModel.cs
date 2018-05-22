using System.ComponentModel.DataAnnotations;
using SFA.DAS.EAS.Application.Commands.SendTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Dtos;

namespace SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitations
{
    public class SendTransferConnectionInvitationViewModel
    {
        [Required(ErrorMessage = "Option required")]
        [RegularExpression("Confirm|ReEnterAccountId", ErrorMessage = "Option required")]
        public string Choice { get; set; }
        
        public AccountDto ReceiverAccount { get; set; }

        public AccountDto SenderAccount { get; set; }

        [Required]
        public SendTransferConnectionInvitationCommand SendTransferConnectionInvitationCommand { get; set; }
    }
}