using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Commands;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers
{
    public class AccountUserRemovedEventHandler : IHandleMessages<AccountUserRolesRemovedEvent>
    {
        private readonly IReadStoreMediator _mediator;

        public AccountUserRemovedEventHandler(IReadStoreMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task Handle(AccountUserRolesRemovedEvent message, IMessageHandlerContext context)
        {
            await _mediator.Send(new RemoveAccountUserCommand(message.AccountId, message.UserRef, context.MessageId, message.Created));
        }
    }
}
