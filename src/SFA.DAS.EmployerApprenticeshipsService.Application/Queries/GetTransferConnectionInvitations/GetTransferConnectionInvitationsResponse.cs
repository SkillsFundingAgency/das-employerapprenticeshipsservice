using System.Collections.Generic;
using SFA.DAS.EAS.Application.Dtos;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitations
{
    public class GetTransferConnectionInvitationsResponse
    {
        public long AccountId { get; set; }
        public IEnumerable<TransferConnectionInvitationDto> TransferConnectionInvitations { get; set; }
    }
}