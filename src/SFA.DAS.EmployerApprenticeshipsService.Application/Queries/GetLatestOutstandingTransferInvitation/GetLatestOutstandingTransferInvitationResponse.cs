using SFA.DAS.EAS.Domain.Models.TransferConnections;

namespace SFA.DAS.EAS.Application.Queries.GetLatestOutstandingTransferInvitation
{
    public class GetLatestOutstandingTransferInvitationResponse
    {
        public TransferConnectionInvitation TransferConnectionInvitation { get; set; }
    }
}