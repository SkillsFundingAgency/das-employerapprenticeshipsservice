﻿using SFA.DAS.NServiceBus;

#pragma warning disable 618
namespace SFA.DAS.EmployerFinance.Messages.Events
{
    public class RefreshPaymentDataCompletedEvent : Event
    {
        public long AccountId { get; set; }
        public string PeriodEnd { get; set; }
        /// <summary>
        /// true if we have processed some payments; otherwsie false;
        /// </summary>
        public bool PaymentsProcessed { get; set; }
    }
}
#pragma warning restore 618