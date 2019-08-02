using SFA.DAS.NServiceBus;

#pragma warning disable 618
namespace SFA.DAS.EmployerFinance.Messages.Commands
{
    public class ImportAccountLevyDeclarationsCommand : Command
    {
        public long AccountId { get; set; }
        public string PayeRef { get; set; }
    }
}
#pragma warning restore 618