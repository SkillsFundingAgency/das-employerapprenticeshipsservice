using SFA.DAS.EAS.Application.Dtos;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitations
{
    public class GetTransferConnectionInvitationsResponse
    {
        public long AccountId { get; set; }
        public IEnumerable<TransferConnectionInvitationDto> TransferConnectionInvitations { get; set; }
    }
}