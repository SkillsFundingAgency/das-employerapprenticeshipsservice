using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Messages.Commands;

namespace SFA.DAS.EmployerFinance.MessageHandlers.CommandHandlers
{
    public class ExpireFundsCommandHandler : IHandleMessages<ExpireFundsCommand>
    {
        private readonly IFundsInRepository _fundsInRepository;
        private readonly IFundsOutRepository _fundsOutRepository;

        public ExpireFundsCommandHandler(IFundsInRepository fundsInRepository, IFundsOutRepository fundsOutRepository)
        {
            _fundsInRepository = fundsInRepository;
            _fundsOutRepository = fundsOutRepository;
        }

        public Task Handle(ExpireFundsCommand message, IMessageHandlerContext context)
        {
            var fundsIn = _fundsInRepository.GetFundsIn(message.AccountId);
            var fundsOut = _fundsOutRepository.GetFundsOut(message.AccountId);

            return Task.CompletedTask;
        }
    }
}
