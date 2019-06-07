using SFA.DAS.NServiceBus;

namespace SFA.DAS.EmployerFinance.Messages.Events
{
    public class AccountFundsExpiredEvent : Event
    {
        public long AccountId { get; set; }
        public short PeriodMonth { get; set; }
        public string PeriodYear { get; set; }
    }
}
