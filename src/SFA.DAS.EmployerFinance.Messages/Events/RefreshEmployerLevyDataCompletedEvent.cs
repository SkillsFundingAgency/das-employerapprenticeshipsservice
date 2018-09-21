using System;
using SFA.DAS.NServiceBus;

namespace SFA.DAS.EmployerFinance.Messages.Events
{
    public class RefreshEmployerLevyDataCompletedEvent : Event
    {
        public long AccountId { get; set; }
        public short PeriodMonth { get; set; }

        public string PeriodYear { get; set; }
        /// <summary>
        /// true if we have imported some levy; otherwise false;
        /// </summary>
        public bool LevyImported { get; set; }
    }
}
