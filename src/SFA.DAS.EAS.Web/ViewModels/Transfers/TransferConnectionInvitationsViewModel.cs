using System.Collections.Generic;
using SFA.DAS.EAS.Application.Dtos;

namespace SFA.DAS.EAS.Web.ViewModels.Transfers
{
    public class TransferConnectionInvitationsViewModel
    {
        public long AccountId { get; set; }
        public IEnumerable<TransferConnectionInvitationDto> TransferConnectionInvitations { get; set; }
    }
}