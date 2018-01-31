using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.TransferConnections;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitations
{
    public class GetTransferConnectionInvitationsResponse
    {
        public IEnumerable<TransferConnectionInvitation> TransferConnectionInvitations { get; set; }
    }
}