using SFA.DAS.EmployerAccounts.DtosX;

namespace SFA.DAS.EmployerAccounts.Queries.SendTransferConnectionInvitation
{
    public class SendTransferConnectionInvitationResponse
    {
        public AccountDto ReceiverAccount { get; set; }

        public AccountDto SenderAccount { get; set; }
    }
}