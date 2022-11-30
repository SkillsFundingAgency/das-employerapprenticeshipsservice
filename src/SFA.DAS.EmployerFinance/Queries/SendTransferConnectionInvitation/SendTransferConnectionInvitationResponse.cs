using SFA.DAS.EmployerFinance.Dtos;

namespace SFA.DAS.EmployerFinance.Queries.SendTransferConnectionInvitation
{
    public class SendTransferConnectionInvitationResponse
    {
        public AccountDto ReceiverAccount { get; set; }
        public AccountDto SenderAccount { get; set; }
    }
}