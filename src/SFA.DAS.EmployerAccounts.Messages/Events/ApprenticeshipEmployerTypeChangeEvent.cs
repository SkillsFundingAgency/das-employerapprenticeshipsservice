using System;
using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Messages.Events
{
    public class ApprenticeshipEmployerTypeChangeEvent 
    {
        public long AccountId { get; set; }
        public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
        public DateTime Created { get; set; }
    }
}
