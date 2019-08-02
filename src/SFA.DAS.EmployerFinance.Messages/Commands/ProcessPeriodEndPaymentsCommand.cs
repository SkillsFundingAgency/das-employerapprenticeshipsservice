using SFA.DAS.NServiceBus;
#pragma warning disable 618
namespace SFA.DAS.EmployerFinance.Messages.Commands
{
    public class ProcessPeriodEndPaymentsCommand : Command
    {
        public string PeriodEndRef { get; set; }
    }
}
#pragma warning restore 618