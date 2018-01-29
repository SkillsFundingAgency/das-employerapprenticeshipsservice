using System.ComponentModel.DataAnnotations;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Models.TransferConnections;

namespace SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitations
{
    public class SentTransferConnectionInvitationViewModel
    {
        [Required(ErrorMessage = "Option required.")]
        [RegularExpression("GoToTransfersPage|GoToHomepage", ErrorMessage = "Option required.")]
        public string Choice { get; set; }

        public Account ReceiverAccount { get; set; }
        public TransferConnectionInvitation TransferConnectionInvitation { get; set; }
    }
}