using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Commands;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers
{
    public class UserRoleUpdatedEventHandler : IHandleMessages<UserRolesUpdatedEvent>
    {
        private readonly IReadStoreMediator _mediator;

        public UserRoleUpdatedEventHandler(IReadStoreMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task Handle(UserRolesUpdatedEvent message, IMessageHandlerContext context)
        {
            await _mediator.Send(new UserRolesUpdatedCommand(message.AccountId, Guid.Parse(message.UserRef), message.Roles, context.MessageId, message.Updated));
        }
    }
}
