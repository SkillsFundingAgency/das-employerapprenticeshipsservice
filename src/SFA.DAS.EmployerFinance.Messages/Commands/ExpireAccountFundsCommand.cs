using SFA.DAS.NServiceBus;

#pragma warning disable 618
namespace SFA.DAS.EmployerFinance.Messages.Commands
{
    public class ExpireAccountFundsCommand : Command
    {
        public long AccountId { get; set; }
    }
}
#pragma warning restore 618