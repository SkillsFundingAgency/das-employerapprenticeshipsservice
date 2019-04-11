using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Messages.Commands;

namespace SFA.DAS.EmployerFinance.MessageHandlers.CommandHandlers
{
    public class ExpireFundsCommandHandler : IHandleMessages<ExpireFundsCommand>
    {
        private readonly ICurrentDateTime _currentDateTime;
        private readonly IEmployerAccountRepository _accountRepository;

        public ExpireFundsCommandHandler(ICurrentDateTime currentDateTime, IEmployerAccountRepository accountRepository)
        {
            _currentDateTime = currentDateTime;
            _accountRepository = accountRepository;
        }

        public async Task Handle(ExpireFundsCommand message, IMessageHandlerContext context)
        {
            var now = _currentDateTime.Now;
            var accounts = await _accountRepository.GetAllAccounts();
            var commands = accounts.Select(a => new ExpireAccountFundsCommand { AccountId = a.Id });

            var tasks = commands.Select(c =>
            {
                var sendOptions = new SendOptions();

                sendOptions.RequireImmediateDispatch();
                sendOptions.RouteToThisEndpoint();
                sendOptions.SetMessageId($"{nameof(ExpireAccountFundsCommand)}-{now.Year}-{now.Month}-{c.AccountId}");

                return context.Send(c, sendOptions);
            });

            await Task.WhenAll(tasks);
        }
    }
}