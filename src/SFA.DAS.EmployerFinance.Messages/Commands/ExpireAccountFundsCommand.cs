using SFA.DAS.NServiceBus;

namespace SFA.DAS.EmployerFinance.Messages.Commands
{
    public class ExpireAccountFundsCommand : Command
    {
        public long AccountId { get; set; }
    }
}
