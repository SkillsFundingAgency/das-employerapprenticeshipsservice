using System;
namespace SFA.DAS.EmployerFinance.Messages.Events
{
    public class RefreshPaymentDataCompletedEvent 
    {
        public long AccountId { get; set; }
        public string PeriodEnd { get; set; }
        /// <summary>
        /// true if we have processed some payments; otherwise false;
        /// </summary>
        public bool PaymentsProcessed { get; set; }
        public DateTime Created { get; set; }
    }
}
