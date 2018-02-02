namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitationAccount
{
    public class GetTransferConnectionInvitationAccountResponse
    {
        public Domain.Data.Entities.Account.Account ReceiverAccount { get; set; }
        public Domain.Data.Entities.Account.Account SenderAccount { get; set; }
    }
}