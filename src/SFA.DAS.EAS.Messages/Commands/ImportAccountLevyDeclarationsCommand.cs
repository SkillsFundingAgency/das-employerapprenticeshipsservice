using NServiceBus;

namespace SFA.DAS.EAS.Messages.Commands
{
    public class ImportAccountLevyDeclarationsCommand : ICommand
    {
        public long AccountId { get; set; }
        public string PayeRef { get; set; }
    }
}
