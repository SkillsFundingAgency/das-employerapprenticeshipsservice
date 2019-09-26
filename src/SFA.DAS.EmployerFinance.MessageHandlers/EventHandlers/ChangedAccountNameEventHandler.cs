using System.Threading.Tasks;
using MediatR;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerFinance.Commands.RenameAccount;

namespace SFA.DAS.EmployerFinance.MessageHandlers.EventHandlers
{
    public class ChangedAccountNameEventHandler : IHandleMessages<ChangedAccountNameEvent>
    {
        private readonly IMediator _mediator;

        public ChangedAccountNameEventHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(ChangedAccountNameEvent message, IMessageHandlerContext context)
        {
            await _mediator.SendAsync(new RenameAccountCommand(message.AccountId, message.CurrentName));
        }
    }
}
