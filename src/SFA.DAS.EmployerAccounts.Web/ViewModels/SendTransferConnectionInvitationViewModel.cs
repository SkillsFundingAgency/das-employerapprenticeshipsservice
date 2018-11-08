using System.ComponentModel.DataAnnotations;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Commands.SendTransferConnectionInvitation;
using SFA.DAS.EmployerAccounts.Dtos;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class SendTransferConnectionInvitationViewModel : MembershipMessage
    {
        [Required(ErrorMessage = "Option required")]
        [RegularExpression("Confirm|ReEnterAccountId", ErrorMessage = "Option required")]
        public string Choice { get; set; }

        public AccountDto ReceiverAccount { get; set; }
        public AccountDto SenderAccount { get; set; }

        [Required]
        [RegularExpression(SFA.DAS.EmployerAccounts.Constants.AccountHashedIdRegex)]
        public string ReceiverAccountPublicHashedId { get; set; }
    }
}