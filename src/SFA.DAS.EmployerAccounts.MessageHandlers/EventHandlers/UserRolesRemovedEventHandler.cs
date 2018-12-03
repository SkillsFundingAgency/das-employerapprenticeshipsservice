using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Commands;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers
{
    public class UserRolesRemovedEventHandler : IHandleMessages<UserRolesRemovedEvent>
    {
        private readonly IReadStoreMediator _mediator;

        public UserRolesRemovedEventHandler(IReadStoreMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task Handle(UserRolesRemovedEvent message, IMessageHandlerContext context)
        {
            await _mediator.Send(new RemoveUserRolesCommand(message.AccountId, message.UserId, context.MessageId, message.Created));
        }
    }
}
