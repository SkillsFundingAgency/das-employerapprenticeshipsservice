using SFA.DAS.Common.Domain.Types;
using SFA.DAS.NServiceBus;

namespace SFA.DAS.EmployerFinance.Messages.Commands
{
    public class SetAccountLevyStatusCommand : Command  
    {
        public long AccountId { get; set; }
        public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; } 
    }
}