using SFA.DAS.EAS.Application.Dtos;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitation
{
    public class GetTransferConnectionInvitationResponse
    {
        public long AccountId { get; set; }
        public TransferConnectionInvitationDto TransferConnectionInvitation { get; set; }
    }
}