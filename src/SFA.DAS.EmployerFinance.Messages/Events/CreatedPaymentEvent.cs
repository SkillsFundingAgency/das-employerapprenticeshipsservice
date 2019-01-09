using System;
namespace SFA.DAS.EmployerFinance.Messages.Events
{
    public class CreatedPaymentEvent 
    {
        public long AccountId { get; set; }
        public decimal Amount { get; set; }
        public string ProviderName { get; set; }
        public DateTime Created { get; set; }
    }
}
