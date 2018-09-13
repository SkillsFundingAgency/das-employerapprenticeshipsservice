using SFA.DAS.NServiceBus;

namespace SFA.DAS.EmployerFinance.Messages.Commands
{
    public class ImportAccountPaymentsCommand : Command
    {
        public long AccountId { get; set; }
        public string PeriodEndRef { get; set; }
    }
}
