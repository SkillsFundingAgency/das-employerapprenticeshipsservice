using NServiceBus;

namespace SFA.DAS.EAS.Messages.Commands
{
    public class ImportAccountPaymentsCommand : ICommand
    {
        public long AccountId { get; set; }
        public string PeriodEndRef { get; set; }
    }
}
