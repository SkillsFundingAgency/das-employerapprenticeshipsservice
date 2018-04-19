using System.ComponentModel.DataAnnotations;
using SFA.DAS.EAS.Application.Queries.SendTransferConnectionInvitation;

namespace SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitations
{
    public class StartTransferConnectionInvitationViewModel : ViewModel<SendTransferConnectionInvitationQuery>
    {
        [Required]
        public SendTransferConnectionInvitationQuery SendTransferConnectionInvitationQuery { get; set; }

        public override void Map(SendTransferConnectionInvitationQuery message)
        {
            SendTransferConnectionInvitationQuery = message;
        }
    }
}