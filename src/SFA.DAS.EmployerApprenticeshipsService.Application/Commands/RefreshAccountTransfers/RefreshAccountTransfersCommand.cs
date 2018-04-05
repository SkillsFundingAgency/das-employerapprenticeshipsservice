using MediatR;

namespace SFA.DAS.EAS.Application.Commands.RefreshAccountTransfers
{
    public class RefreshAccountTransfersCommand : IAsyncRequest
    {
        public long ReceiverAccountId { get; set; }
        public string PeriodEnd { get; set; }
    }
}
