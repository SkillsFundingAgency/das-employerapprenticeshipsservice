using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.EmployerFinance.Types.Models;
using SFA.DAS.EmployerFinance.Extensions;

namespace SFA.DAS.EmployerFinance.MessageHandlers.CommandHandlers
{
    public class ExpireFundsCommandHandler : IHandleMessages<ExpireFundsCommand>
    {
        private readonly IFundsInRepository _fundsInRepository;
        private readonly IFundsOutRepository _fundsOutRepository;
        private readonly IExpiredFunds _expiredFunds;
        private readonly IExpiredFundsRepository _expiredFundsRepository;

        public ExpireFundsCommandHandler(IFundsInRepository fundsInRepository, IFundsOutRepository fundsOutRepository,
            IExpiredFunds expiredFunds, IExpiredFundsRepository expiredFundsRepository)
        {
            _fundsInRepository = fundsInRepository;
            _fundsOutRepository = fundsOutRepository;
            _expiredFunds = expiredFunds;
            _expiredFundsRepository = expiredFundsRepository;
        }

        public async Task Handle(ExpireFundsCommand message, IMessageHandlerContext context)
        {
            var fundsIn = await _fundsInRepository.GetFundsIn(message.AccountId);
            var cpdFundsIn = fundsIn.ToCalendarPeriodDictionary();
            var fundsOut = await _fundsOutRepository.GetFundsOut(message.AccountId);
            var existingExpiredFunds = await _expiredFundsRepository.Get(message.AccountId);
            _expiredFunds.GetExpiringFunds(fundsIn.ToCalendarPeriodDictionary(), fundsOut.ToCalendarPeriodDictionary(),
                existingExpiredFunds.ToCalendarPeriodDictionary(), 24);
        }
    }
}
