using SFA.DAS.NServiceBus;

namespace SFA.DAS.EmployerFinance.Messages.Commands
{
    public class ExpireFundsCommand : Command
    {
        public int Month { get; set; }
        public int Year { get; set; }
    }
}