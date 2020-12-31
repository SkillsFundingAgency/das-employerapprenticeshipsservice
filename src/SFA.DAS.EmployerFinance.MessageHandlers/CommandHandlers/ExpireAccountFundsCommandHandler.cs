﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.EmployerFinance.Types.Models;
using SFA.DAS.EmployerFinance.Extensions;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.MessageHandlers.CommandHandlers
{
    public class ExpireAccountFundsCommandHandler : IHandleMessages<ExpireAccountFundsCommand>
    {
        private readonly ICurrentDateTime _currentDateTime;
        private readonly ILevyFundsInRepository _levyFundsInRepository;
        private readonly IPaymentFundsOutRepository _paymentFundsOutRepository;
        private readonly IExpiredFunds _expiredFunds;
        private readonly IExpiredFundsRepository _expiredFundsRepository;
        private readonly ILog _logger;
        private readonly EmployerFinanceConfiguration _configuration;

        public ExpireAccountFundsCommandHandler(
            ICurrentDateTime currentDateTime,
            ILevyFundsInRepository levyFundsInRepository,
            IPaymentFundsOutRepository paymentFundsOutRepository,
            IExpiredFunds expiredFunds,
            IExpiredFundsRepository expiredFundsRepository,
            ILog logger,
            EmployerFinanceConfiguration configuration)
        {
            _currentDateTime = currentDateTime;
            _levyFundsInRepository = levyFundsInRepository;
            _paymentFundsOutRepository = paymentFundsOutRepository;
            _expiredFunds = expiredFunds;
            _expiredFundsRepository = expiredFundsRepository;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task Handle(ExpireAccountFundsCommand message, IMessageHandlerContext context)
        {
            _logger.Info($"Expiring funds for account ID '{message.AccountId}' with expiry period '{_configuration.FundsExpiryPeriod}'");

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var now = _currentDateTime.Now;
            var fundsIn = await _levyFundsInRepository.GetLevyFundsIn(message.AccountId);
            var fundsOut = await _paymentFundsOutRepository.GetPaymentFundsOut(message.AccountId);
            var existingExpiredFunds = await _expiredFundsRepository.Get(message.AccountId);

            var expiredFunds = _expiredFunds.GetExpiredFunds(
                fundsIn.ToCalendarPeriodDictionary(),
                fundsOut.ToCalendarPeriodDictionary(),
                existingExpiredFunds.ToCalendarPeriodDictionary(),
                _configuration.FundsExpiryPeriod,
                now);

            var currentCalendarPeriod = new CalendarPeriod(_currentDateTime.Now.Year, _currentDateTime.Now.Month);
            if (!expiredFunds.ContainsKey(currentCalendarPeriod))
            {
                expiredFunds.Add(currentCalendarPeriod, 0);
            }

            await _expiredFundsRepository.Create(message.AccountId, expiredFunds.ToExpiredFundsList(), now);

            //todo: do we publish the event if no fund expired? we could add a bool like the levy declared message
            // once an account has an expired fund, we'll publish every run, even if no additional funds have expired
            if (expiredFunds.Any(ef => ef.Value != 0m))
                await PublishAccountFundsExpiredEvent(context, message.AccountId);

            stopWatch.Stop();

            _logger.Info($"Expired '{expiredFunds.Count}' month(s) of funds for account ID '{message.AccountId}' with expiry period '{_configuration.FundsExpiryPeriod}' in {stopWatch.Elapsed.TotalSeconds} seconds");
        }

        private async Task PublishAccountFundsExpiredEvent(IMessageHandlerContext context, long accountId)
        {
            await context.Publish(new AccountFundsExpiredEvent
            {
                AccountId = accountId,
                Created = DateTime.UtcNow
            });
        }
    }
}
