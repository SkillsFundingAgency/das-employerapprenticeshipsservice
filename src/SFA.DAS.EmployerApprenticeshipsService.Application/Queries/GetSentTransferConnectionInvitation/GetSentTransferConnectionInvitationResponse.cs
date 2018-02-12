using SFA.DAS.EAS.Domain.Models.TransferConnections;

namespace SFA.DAS.EAS.Application.Queries.GetSentTransferConnectionInvitation
{
    public class GetSentTransferConnectionInvitationResponse
    {
        public Domain.Data.Entities.Account.Account ReceiverAccount { get; set; }
        public TransferConnectionInvitation TransferConnectionInvitation { get; set; }
    }
}