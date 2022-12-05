using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Messages.Commands;

namespace SFA.DAS.EmployerFinance.MessageHandlers.CommandHandlers
{
    public class DraftExpireFundsCommandHandler : IHandleMessages<DraftExpireFundsCommand>
    {
        private readonly ICurrentDateTime _currentDateTime;
        private readonly IEmployerAccountRepository _accountRepository;

        public DraftExpireFundsCommandHandler(ICurrentDateTime currentDateTime, IEmployerAccountRepository accountRepository)
        {
            _currentDateTime = currentDateTime;
            _accountRepository = accountRepository;
        }
        public async Task Handle(DraftExpireFundsCommand message, IMessageHandlerContext context)
        {
            var now = _currentDateTime.Now;
            var accounts = await _accountRepository.GetAll();
            
            var messageTasks = new List<Task>();
            var sendCounter = 0;

            foreach (var account in accounts)
            {
                var sendOptions = new SendOptions();
                sendOptions.RouteToThisEndpoint();

                messageTasks.Add(context.Send(new DraftExpireAccountFundsCommand { AccountId = account.Id, DateTo = message.DateTo }, sendOptions));
                sendCounter++;

                if (sendCounter % 1000 == 0)
                {
                    await Task.WhenAll(messageTasks);
                    messageTasks.Clear();
                    await Task.Delay(500);
                }
            }

            // await final tasks not % 1000
            await Task.WhenAll(messageTasks);
        }
    }
}