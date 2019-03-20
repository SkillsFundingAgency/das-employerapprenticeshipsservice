using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Messages.Commands;

namespace SFA.DAS.EmployerFinance.MessageHandlers.CommandHandlers
{
    public class ExpireFundsCommandHandler : IHandleMessages<ExpireFundsCommand>
    {
        private readonly IEmployerAccountRepository _accountRepository;

        public ExpireFundsCommandHandler(IEmployerAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task Handle(ExpireFundsCommand message, IMessageHandlerContext context)
        {
            var accounts = await _accountRepository.GetAllAccounts();
            var commands = accounts.Select(a => new ExpireAccountFundsCommand { Year = message.Year, Month = message.Month, AccountId = a.Id });

            var tasks = commands.Select(c =>
            {
                var sendOptions = new SendOptions();

                sendOptions.RequireImmediateDispatch();
                sendOptions.RouteToThisEndpoint();
                sendOptions.SetMessageId($"{c.Year}-{c.Month}-{c.AccountId}");

                return context.Send(c, sendOptions);
            });

            await Task.WhenAll(tasks);
        }
    }
}