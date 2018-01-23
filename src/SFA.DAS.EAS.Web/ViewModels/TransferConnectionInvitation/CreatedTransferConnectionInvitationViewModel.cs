using System.ComponentModel.DataAnnotations;
using SFA.DAS.EAS.Application.Commands.SendTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitation
{
    public class CreatedTransferConnectionInvitationViewModel : ViewModel<SendTransferConnectionInvitationCommand>
    {
        [Required(ErrorMessage = "Option required.")]
        [RegularExpression("Confirm|GoToTransfersPage", ErrorMessage = "Option required.")]
        public string Choice { get; set; }
        
        public Domain.Data.Entities.Account.Account ReceiverAccount { get; set; }
        public Domain.Models.TransferConnection.TransferConnectionInvitation TransferConnectionInvitation { get; set; }
    }
}