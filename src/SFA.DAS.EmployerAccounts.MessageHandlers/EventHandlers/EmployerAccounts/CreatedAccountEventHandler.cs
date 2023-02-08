using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Commands;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;
using SFA.DAS.EmployerAccounts.Types.Models;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers.EmployerAccounts
{
    public class CreatedAccountEventHandler : IHandleMessages<CreatedAccountEvent>
    {
        private readonly IReadStoreMediator _mediator;
        private readonly ILog _logger;

        public CreatedAccountEventHandler(IReadStoreMediator mediator, ILog logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        public async Task Handle(CreatedAccountEvent message, IMessageHandlerContext context)
        {
            _logger.Info($"{nameof(CreatedAccountEvent)} received for Account: {message.HashedId}");

            await _mediator.Send(new CreateAccountUserCommand(message.AccountId, message.UserRef, UserRole.Owner, context.MessageId, message.Created));
        }
    }
}