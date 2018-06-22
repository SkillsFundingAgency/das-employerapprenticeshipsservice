using NServiceBus;

namespace SFA.DAS.EAS.Messages.Commands
{
    public interface IImportAccountPaymentsCommand : ICommand
    {
        long AccountId { get; set; }
        string PeriodEndRef { get; set; }
    }
}
