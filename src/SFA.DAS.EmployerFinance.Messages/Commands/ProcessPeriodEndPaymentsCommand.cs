using SFA.DAS.NServiceBus;

namespace SFA.DAS.EmployerFinance.Messages.Commands
{
    public class ProcessPeriodEndPaymentsCommand : Command
    {
        public string PeriodEndRef { get; set; }
    }
}
