using SFA.DAS.EAS.Application.Dtos;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Web.ViewModels.Transfers
{
    public class TransferConnectionInvitationsViewModel
    {
        public long AccountId { get; set; }
        public decimal TransferAllowance { get; set; }
        public IEnumerable<TransferConnectionInvitationDto> TransferConnectionInvitations { get; set; }
    }
}