using System;
using SFA.DAS.NServiceBus;

namespace SFA.DAS.EmployerFinance.Messages.Commands
{
    public class DraftExpireFundsCommand : Command
    {
        public DateTime? DateTo { get; set; }
    }
}