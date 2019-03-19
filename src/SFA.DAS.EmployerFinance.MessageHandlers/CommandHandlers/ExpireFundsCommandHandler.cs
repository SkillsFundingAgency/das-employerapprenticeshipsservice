using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerFinance.Messages.Commands;

namespace SFA.DAS.EmployerFinance.MessageHandlers.CommandHandlers
{
    public class ExpireFundsCommandHandler : IHandleMessages<ExpireFundsCommand>
    {
        public Task Handle(ExpireFundsCommand message, IMessageHandlerContext context)
        {
            throw new NotImplementedException();
        }
    }
}
