﻿using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Commands;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers.EmployerAccounts
{
    public class AccountUserRolesUpdatedEventHandler : IHandleMessages<AccountUserRolesUpdatedEvent>
    {
        private readonly IReadStoreMediator _mediator;

        public AccountUserRolesUpdatedEventHandler(IReadStoreMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task Handle(AccountUserRolesUpdatedEvent message, IMessageHandlerContext context)
        {
            await _mediator.Send(new UpdateAccountUserCommand(message.AccountId, message.UserRef, message.Role, context.MessageId, message.Created));
        }
    }
}
