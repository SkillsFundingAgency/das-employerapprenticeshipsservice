using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.MessageHandlers.CommandHandlers
{
    public class DraftExpireFundsCommandHandler : IHandleMessages<DraftExpireFundsCommand>
    {
        private readonly ICurrentDateTime _currentDateTime;
        private readonly IEmployerAccountRepository _accountRepository;
        private readonly ILog _logger;

        public DraftExpireFundsCommandHandler(ICurrentDateTime currentDateTime, IEmployerAccountRepository accountRepository, ILog logger)
        {
            _currentDateTime = currentDateTime;
            _accountRepository = accountRepository;
            _logger = logger;
        }
        public async Task Handle(DraftExpireFundsCommand message, IMessageHandlerContext context)
        {
            try
            {
                var now = _currentDateTime.Now;
                var accounts = await _accountRepository.GetAll();

                var messageTasks = new List<Task>();
                var sendCounter = 0;

                _logger.Info($"Queueing {nameof(DraftExpireAccountFundsCommand)} messages for {accounts.Count} accounts.");

                foreach (var account in accounts)
                {
                    var sendOptions = new SendOptions();
                    sendOptions.RouteToThisEndpoint();
                    sendOptions.RequiredImmediateDispatch();

                    messageTasks.Add(context.Send(new DraftExpireAccountFundsCommand { AccountId = account.Id, DateTo = message.DateTo }, sendOptions));
                    sendCounter++;

                    if (sendCounter % 1000 == 0)
                    {
                        await Task.WhenAll(messageTasks).ConfigureAwait(false); ;
                        _logger.Info($"Queued {sendCounter} of {accounts.Count} messages.");
                        messageTasks.Clear();
                        await Task.Delay(500).ConfigureAwait(false); ;
                    }
                }

                // await final tasks not % 1000
                await Task.WhenAll(messageTasks).ConfigureAwait(false);

                _logger.Info($"{nameof(DraftExpireFundsCommandHandler)} completed.");
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"{nameof(DraftExpireFundsCommandHandler)} failed");
            }
        }
    }
}