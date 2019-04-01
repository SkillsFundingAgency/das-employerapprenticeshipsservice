using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.EmployerFinance.Types.Models;
using SFA.DAS.EmployerFinance.Extensions;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.MessageHandlers.CommandHandlers
{
    public class ExpireAccountFundsCommandHandler : IHandleMessages<ExpireAccountFundsCommand>
    {
        private readonly ILevyFundsInRepository _levyFundsInRepository;
        private readonly IPaymentFundsOutRepository _paymentFundsOutRepository;
        private readonly IExpiredFunds _expiredFunds;
        private readonly IExpiredFundsRepository _expiredFundsRepository;
        private readonly ILog _logger;

        public ExpireAccountFundsCommandHandler(
            ILevyFundsInRepository levyFundsInRepository,
            IPaymentFundsOutRepository paymentFundsOutRepository,
            IExpiredFunds expiredFunds,
            IExpiredFundsRepository expiredFundsRepository,
            ILog logger)
        {
            _levyFundsInRepository = levyFundsInRepository;
            _paymentFundsOutRepository = paymentFundsOutRepository;
            _expiredFunds = expiredFunds;
            _expiredFundsRepository = expiredFundsRepository;
            _logger = logger;
        }

        public async Task Handle(ExpireAccountFundsCommand message, IMessageHandlerContext context)
        {
            _logger.Info($"Expiring funds for account ID '{message.AccountId}'");

            var fundsIn = await _levyFundsInRepository.GetLevyFundsIn(message.AccountId);
            var fundsOut = await _paymentFundsOutRepository.GetPaymentFundsOut(message.AccountId);
            var existingExpiredFunds = await _expiredFundsRepository.Get(message.AccountId);

            var fundsToExpire = _expiredFunds.GetExpiringFunds(
                fundsIn.ToCalendarPeriodDictionary(),
                fundsOut.ToCalendarPeriodDictionary(),
                existingExpiredFunds.ToCalendarPeriodDictionary(),
                24);

            await _expiredFundsRepository.Create(message.AccountId, fundsToExpire.ToExpiredFundsEntityList());

            _logger.Info($"Expired funds for account ID '{message.AccountId}'");
        }
    }
}
