using MediatR;

namespace SFA.DAS.EAS.Application.Commands.CreateTransferTransactions
{
    public class CreateTransferTransactionsCommand : IAsyncRequest
    {
        public long ReceiverAccountId { get; set; }
        public string PeriodEnd { get; set; }
    }
}
