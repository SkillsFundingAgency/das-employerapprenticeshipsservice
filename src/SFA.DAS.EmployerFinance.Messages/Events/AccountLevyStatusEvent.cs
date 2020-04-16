using SFA.DAS.Common.Domain.Types;
using SFA.DAS.NServiceBus;

namespace SFA.DAS.EmployerFinance.Messages.Events
{
    public class AccountLevyStatusEvent : Event
    {
        public long AccountId { get; set; }
        public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; } 
    }
}