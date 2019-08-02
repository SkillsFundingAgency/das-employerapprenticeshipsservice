using SFA.DAS.NServiceBus;

#pragma warning disable 618
namespace SFA.DAS.EmployerFinance.Messages.Commands
{
    public class ImportAccountPaymentsCommand : Command
    {
        public long AccountId { get; set; }
        public string PeriodEndRef { get; set; }
    }
}
#pragma warning restore 618