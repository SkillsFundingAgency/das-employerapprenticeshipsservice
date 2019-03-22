using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.EmployerFinance.Types.Models;
using SFA.DAS.EmployerFinance.Extensions;

namespace SFA.DAS.EmployerFinance.MessageHandlers.CommandHandlers
{
    public class ExpireAccountFundsCommandHandler : IHandleMessages<ExpireAccountFundsCommand>
    {
        private readonly ILevyFundsInRepository _levyFundsInRepository;
        private readonly IPaymentFundsOutRepository _paymentFundsOutRepository;
        private readonly IExpiredFunds _expiredFunds;
        private readonly IExpiredFundsRepository _expiredFundsRepository;

        public ExpireAccountFundsCommandHandler(ILevyFundsInRepository fundsInRepository, IPaymentFundsOutRepository fundsOutRepository,
            IExpiredFunds expiredFunds, IExpiredFundsRepository expiredFundsRepository)
        {
            _levyFundsInRepository = fundsInRepository;
            _paymentFundsOutRepository = fundsOutRepository;
            _expiredFunds = expiredFunds;
            _expiredFundsRepository = expiredFundsRepository;
        }

        public async Task Handle(ExpireAccountFundsCommand message, IMessageHandlerContext context)
        {
            var fundsIn = await _levyFundsInRepository.GetLevyFundsIn(message.AccountId);
            var fundsOut = await _paymentFundsOutRepository.GetPaymentFundsOut(message.AccountId);
            var existingExpiredFunds = await _expiredFundsRepository.Get(message.AccountId);
            var fundsToExpire = _expiredFunds.GetExpiringFunds(fundsIn.ToCalendarPeriodDictionary(), fundsOut.ToCalendarPeriodDictionary(),
                existingExpiredFunds.ToCalendarPeriodDictionary(), 24);
            await _expiredFundsRepository.Create(message.AccountId, fundsToExpire.ToExpiredFundsEntityList());
        }
    }
}
