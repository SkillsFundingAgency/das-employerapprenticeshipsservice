using SFA.DAS.EmployerAccounts.DtosX;

namespace SFA.DAS.EmployerAccounts.Queries.GetTransferConnectionInvitation
{
    public class GetTransferConnectionInvitationResponse
    {
        public long AccountId { get; set; }
        public TransferConnectionInvitationDto TransferConnectionInvitation { get; set; }
    }
}