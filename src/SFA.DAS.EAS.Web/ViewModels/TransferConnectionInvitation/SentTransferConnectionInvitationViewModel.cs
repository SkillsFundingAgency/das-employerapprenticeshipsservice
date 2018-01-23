using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitation
{
    public class SentTransferConnectionInvitationViewModel
    {
        [Required(ErrorMessage = "Option required.")]
        [RegularExpression("GoToTransfersPage|GoToHomepage", ErrorMessage = "Option required.")]
        public string Choice { get; set; }

        public Domain.Data.Entities.Account.Account ReceiverAccount { get; set; }
        public Domain.Models.TransferConnection.TransferConnectionInvitation TransferConnectionInvitation { get; set; }
    }
}