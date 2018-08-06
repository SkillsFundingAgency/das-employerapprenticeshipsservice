using MediatR;

namespace SFA.DAS.EmployerFinance.Commands.CreateTransferTransactions
{
    public class CreateTransferTransactionsCommand : IAsyncRequest
    {
        public long ReceiverAccountId { get; set; }
        public string PeriodEnd { get; set; }
    }
}
