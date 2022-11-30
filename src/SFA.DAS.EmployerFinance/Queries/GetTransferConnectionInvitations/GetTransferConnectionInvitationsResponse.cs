using System.Collections.Generic;
using SFA.DAS.EmployerFinance.Dtos;

namespace SFA.DAS.EmployerFinance.Queries.GetTransferConnectionInvitations
{
    public class GetTransferConnectionInvitationsResponse
    {
        public IEnumerable<TransferConnectionInvitationDto> TransferConnectionInvitations { get; set; }
        public long AccountId { get; set; }
    }
}