using MediatR;

namespace SFA.DAS.EmployerFinance.Events.ProcessPayment
{
    public class ProcessPaymentEvent : IAsyncNotification
    {
        public long AccountId { get; set; }
    }
}