using SFA.DAS.EAS.Domain.Models.TransferConnection;

namespace SFA.DAS.EAS.Application.Queries.GetSentTransferConnectionInvitationQuery
{
    public class GetSentTransferConnectionInvitationResponse
    {
        public Domain.Data.Entities.Account.Account ReceiverAccount { get; set; }
        public TransferConnectionInvitation TransferConnectionInvitation { get; set; }
    }
}