using System.Threading.Tasks;
using MediatR;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerFinance.Commands.CreateAccount;

namespace SFA.DAS.EmployerFinance.MessageHandlers.EventHandlers
{
    public class CreatedAccountEventHandler : IHandleMessages<CreatedAccountEvent>
    {
        private readonly IMediator _mediator;

        public CreatedAccountEventHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(CreatedAccountEvent message, IMessageHandlerContext context)
        {
            await _mediator.SendAsync(new CreateAccountCommand(message.AccountId, message.Name));
        }
    }
}
