using System;
using SFA.DAS.NServiceBus;

namespace SFA.DAS.EmployerFinance.Messages.Commands
{
    public class DraftExpireAccountFundsCommand : Command
    {
        public long AccountId { get; set; }
        public DateTime? DateTo { get; set; }
    }
}