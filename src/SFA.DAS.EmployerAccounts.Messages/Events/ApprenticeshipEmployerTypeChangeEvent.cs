using System;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.NServiceBus;

namespace SFA.DAS.EmployerAccounts.Messages.Events
{
    public class ApprenticeshipEmployerTypeChangeEvent : Event
    {
        public long AccountId { get; set; }
        public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
    }
}
