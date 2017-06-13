using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Domain.Attributes;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.Messaging;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.LevyAccountUpdater.WebJob.Updater
{
    public class AccountUpdater : IAccountUpdater
    {
        private const string ServiceName = "SFA.DAS.EAS.LevyAccountUpdater";
        private readonly IEmployerAccountRepository _accountRepository;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILog _logger;
        private readonly IEmployerSchemesRepository _employerSchemesRepository;

        [QueueName]
        public string get_employer_levy { get; set; }

        public AccountUpdater(IEmployerAccountRepository accountRepository, IMessagePublisher messagePublisher, ILog logger, IEmployerSchemesRepository employerSchemesRepository)
        {
            _accountRepository = accountRepository;
            _messagePublisher = messagePublisher;
            _logger = logger;
            _employerSchemesRepository = employerSchemesRepository;
        }

        public async Task RunUpdate()
        {
            _logger.Info($"{ServiceName}: Running update schedule");

            try
            {
                var timer = Stopwatch.StartNew();
               
                var employerAccounts = await _accountRepository.GetAllAccounts();

                _logger.Debug($"{ServiceName}: Updating {employerAccounts.Count} levy accounts");

                var tasks = new List<Task>();

                foreach (var account in employerAccounts)
                {
                    var schemes = await _employerSchemesRepository.GetSchemesByEmployerId(account.Id);

                    if (schemes?.SchemesList == null)
                    {
                        continue;
                    }

                    foreach (var scheme in schemes.SchemesList)
                    {
                        _logger.Trace($"{ServiceName}: Creating update levy account message for account {account.Name} (ID: {account.Id}) scheme {scheme.Ref}");
                        tasks.Add(_messagePublisher.PublishAsync(new EmployerRefreshLevyQueueMessage { AccountId = account.Id, PayeRef = scheme.Ref}));
                    }
                }
                
                await Task.WhenAll(tasks);

                _logger.Info($"{ServiceName}: update schedule completed in {timer.Elapsed:g} (hh:mm:ss:ms)");
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error updating levy accounts");
                throw;
            }
        }
    }
}
