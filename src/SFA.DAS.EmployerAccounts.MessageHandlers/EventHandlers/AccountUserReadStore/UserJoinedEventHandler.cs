using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Commands;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers.AccountUserReadStore
{
    public class UserJoinedEventHandler : IHandleMessages<UserJoinedEvent>
    {
        private readonly IReadStoreMediator _mediator;

        public UserJoinedEventHandler(IReadStoreMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task Handle(UserJoinedEvent message, IMessageHandlerContext context)
        {
            await _mediator.Send(new CreateAccountUserCommand(message.AccountId, message.UserRef, message.Roles, context.MessageId, message.Created));
        }
    }
}