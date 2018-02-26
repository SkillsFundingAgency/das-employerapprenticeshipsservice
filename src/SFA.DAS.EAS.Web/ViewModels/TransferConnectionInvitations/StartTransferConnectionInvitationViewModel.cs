using System.ComponentModel.DataAnnotations;
using SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitationAccount;

namespace SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitations
{
    public class StartTransferConnectionInvitationViewModel : ViewModel<GetTransferConnectionInvitationAccountQuery>
    {
        [Required]
        public GetTransferConnectionInvitationAccountQuery GetTransferConnectionInvitationAccountQuery { get; set; }

        public override void Map(GetTransferConnectionInvitationAccountQuery message)
        {
            GetTransferConnectionInvitationAccountQuery = message;
        }
    }
}