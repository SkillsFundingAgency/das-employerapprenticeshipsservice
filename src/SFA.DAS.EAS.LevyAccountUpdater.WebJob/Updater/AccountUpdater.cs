using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Domain.Attributes;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.Messaging;

namespace SFA.DAS.EAS.LevyAccountUpdater.WebJob.Updater
{
    public class AccountUpdater : IAccountUpdater
    {
        private const string ServiceName = "SFA.DAS.EAS.LevyAccountUpdater";
        private readonly IEmployerAccountRepository _accountRepository;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger _logger;

        [QueueName]
        public string get_employer_levy { get; set; }

        public AccountUpdater(IEmployerAccountRepository accountRepository, IMessagePublisher messagePublisher,
            ILogger logger)
        {
            _accountRepository = accountRepository;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async Task RunUpdate()
        {
            _logger.Info($"{ServiceName}: Running update schedule");

            try
            {
                var timer = Stopwatch.StartNew();
               
                var employerAccounts = await _accountRepository.GetAllAccounts();

                _logger.Debug($"{ServiceName}: Updating {employerAccounts.Count} levy accounts");

                var tasks = employerAccounts.Select(
                    x =>
                    {
                        _logger.Trace($"{ServiceName}: Creating update levy account message for account {x.Name} (ID: {x.Id})");
                        return _messagePublisher.PublishAsync(new EmployerRefreshLevyQueueMessage {AccountId = x.Id});
                    }).ToArray();

                await Task.WhenAll(tasks);

                _logger.Info($"{ServiceName}: update schedule completed in {timer.Elapsed:g} (hh:mm:ss:ms)");
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }
    }
}
