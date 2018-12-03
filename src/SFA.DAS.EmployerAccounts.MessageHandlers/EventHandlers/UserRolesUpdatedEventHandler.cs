using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Commands;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers
{
    public class UserRolesUpdatedEventHandler : IHandleMessages<UserRolesUpdatedEvent>
    {
        private readonly IReadStoreMediator _mediator;

        public UserRolesUpdatedEventHandler(IReadStoreMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task Handle(UserRolesUpdatedEvent message, IMessageHandlerContext context)
        {
            await _mediator.Send(new UpdateAccountUserCommand(message.AccountId, message.UserRef, message.UserId, message.Roles, context.MessageId, message.Created));
        }
    }
}
