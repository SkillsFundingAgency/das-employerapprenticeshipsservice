using System.ComponentModel.DataAnnotations;
using SFA.DAS.EmployerAccounts.Commands.SendTransferConnectionInvitation;
using SFA.DAS.EmployerAccounts.Dtos;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
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