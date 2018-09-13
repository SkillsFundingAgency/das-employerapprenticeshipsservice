using MediatR;

namespace SFA.DAS.EmployerFinance.Commands.RefreshAccountTransfers
{
    public class RefreshAccountTransfersCommand : IAsyncRequest
    {
        public long ReceiverAccountId { get; set; }
        public string PeriodEnd { get; set; }
    }
}
