using System.ComponentModel.DataAnnotations;
using SFA.DAS.EAS.Application.Queries.SendTransferConnectionInvitation;

namespace SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitations
{
    public class StartTransferConnectionInvitationViewModel
    {
        [Required]
        public SendTransferConnectionInvitationQuery SendTransferConnectionInvitationQuery { get; set; }
    }
}