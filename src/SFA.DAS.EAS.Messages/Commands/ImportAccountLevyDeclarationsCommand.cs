using SFA.DAS.NServiceBus;

namespace SFA.DAS.EAS.Messages.Commands
{
    public class ImportAccountLevyDeclarationsCommand : Command
    {
        public long AccountId { get; set; }
        public string PayeRef { get; set; }
    }
}
