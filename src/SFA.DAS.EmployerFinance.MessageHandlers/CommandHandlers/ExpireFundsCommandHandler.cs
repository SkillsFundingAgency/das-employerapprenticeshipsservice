using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.MessageHandlers.CommandHandlers
{
    public class ExpireFundsCommandHandler : IHandleMessages<ExpireFundsCommand>
    {
        private readonly ICurrentDateTime _currentDateTime;
        private readonly IEmployerAccountRepository _accountRepository;
        private readonly ILog _logger;


        public ExpireFundsCommandHandler(ICurrentDateTime currentDateTime, IEmployerAccountRepository accountRepository, ILog logger)
        {
            _currentDateTime = currentDateTime;
            _accountRepository = accountRepository;
            _logger = logger;
        }

        public async Task Handle(ExpireFundsCommand message, IMessageHandlerContext context)
        {
            var stopWatch = new Stopwatch();
            var now = _currentDateTime.Now;
            var accounts = await _accountRepository.GetAllAccounts();
            var commands = accounts.Select(a => new ExpireAccountFundsCommand { AccountId = a.Id });

            _logger.Info($"Creating ExpireFundsCommand for {accounts.Count} accounts");
            stopWatch.Start();

            var tasks = commands.Select(c =>
            {
                _logger.Info($"Created ExpireFundsCommand for account {c.AccountId}");

                var sendOptions = new SendOptions();

                sendOptions.RequireImmediateDispatch();
                sendOptions.RouteToThisEndpoint();
                sendOptions.SetMessageId($"{nameof(ExpireAccountFundsCommand)}-{now.Year}-{now.Month}-{c.AccountId}");

                return context.Send(c, sendOptions);
            });

            await Task.WhenAll(tasks);

            stopWatch.Stop();
            _logger.Info($"Finished creating ExpireFundsCommand for {accounts.Count} accounts in {stopWatch.Elapsed.TotalMinutes}:{stopWatch.Elapsed.Seconds}");
        }
    }
}