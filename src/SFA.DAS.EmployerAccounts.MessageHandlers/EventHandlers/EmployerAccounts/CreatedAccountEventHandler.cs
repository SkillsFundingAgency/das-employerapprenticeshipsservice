using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Commands;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers.EmployerAccounts
{
    public class CreatedAccountEventHandler : IHandleMessages<CreatedAccountEvent>
    {
        private readonly IReadStoreMediator _mediator;

        public CreatedAccountEventHandler(IReadStoreMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task Handle(CreatedAccountEvent message, IMessageHandlerContext context)
        {
            await _mediator.Send(new CreateAccountUserCommand(message.AccountId, message.UserRef, UserRole.Owner, context.MessageId, message.Created));
        }
    }
}