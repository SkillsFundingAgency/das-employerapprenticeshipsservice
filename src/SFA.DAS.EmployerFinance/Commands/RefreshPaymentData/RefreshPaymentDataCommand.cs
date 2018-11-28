using MediatR;

namespace SFA.DAS.EmployerFinance.Commands.RefreshPaymentData
{
    public class RefreshPaymentDataCommand : IAsyncRequest
    {
        public long AccountId { get; set; }
        public string PaymentUrl { get; set; }
        public string PeriodEnd { get; set; }
    }
}
