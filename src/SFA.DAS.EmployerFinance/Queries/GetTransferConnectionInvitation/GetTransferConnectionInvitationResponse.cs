using SFA.DAS.EmployerFinance.Dtos;

namespace SFA.DAS.EmployerFinance.Queries.GetTransferConnectionInvitation
{
    public class GetTransferConnectionInvitationResponse
    {
        public long AccountId { get; set; }
        public TransferConnectionInvitationDto TransferConnectionInvitation { get; set; }
    }
}