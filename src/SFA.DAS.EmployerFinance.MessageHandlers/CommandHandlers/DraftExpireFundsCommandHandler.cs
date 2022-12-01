using System.Linq;
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
            var commands = accounts.Select(a => new DraftExpireAccountFundsCommand { AccountId = a.Id, DateTo = message.DateTo});

            var tasks = commands.Select(c =>
            {
                var sendOptions = new SendOptions();

                sendOptions.RequireImmediateDispatch();
                sendOptions.RouteToThisEndpoint();

                return context.Send(c, sendOptions);
            });

            await Task.WhenAll(tasks);
        }
    }
}