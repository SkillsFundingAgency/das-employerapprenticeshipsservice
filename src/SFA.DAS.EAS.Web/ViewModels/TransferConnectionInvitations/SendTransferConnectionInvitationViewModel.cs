using System.ComponentModel.DataAnnotations;
using SFA.DAS.EAS.Application.Commands.SendTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Domain.Data.Entities.Account;

namespace SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitations
{
    public class SendTransferConnectionInvitationViewModel : ViewModel<SendTransferConnectionInvitationCommand>
    {
        [Required(ErrorMessage = "Option required.")]
        [RegularExpression("Confirm|GoToTransfersPage", ErrorMessage = "Option required.")]
        public string Choice { get; set; }
        
        public Account ReceiverAccount { get; set; }
        public Account SenderAccount { get; set; }
    }
}