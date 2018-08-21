using SFA.DAS.EmployerAccounts.Dtos;

namespace SFA.DAS.EmployerAccounts.Queries.GetTransferConnectionInvitation
{
    public class GetTransferConnectionInvitationResponse
    {
        public long AccountId { get; set; }
        public TransferConnectionInvitationDto TransferConnectionInvitation { get; set; }
    }
}