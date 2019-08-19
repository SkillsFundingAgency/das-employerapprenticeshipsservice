using System.Threading.Tasks;
using MediatR;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerFinance.Commands.CreateAccountLegalEntity;

namespace SFA.DAS.EmployerFinance.MessageHandlers.EventHandlers
{
    public class AddedLegalEntityEventHandler : IHandleMessages<AddedLegalEntityEvent>
    {
        private readonly IMediator _mediator;

        public AddedLegalEntityEventHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task Handle(AddedLegalEntityEvent message, IMessageHandlerContext context)
        {
            return _mediator.SendAsync(new CreateAccountLegalEntityCommand(message.AccountLegalEntityId, null, null, null,
                message.AccountId, message.LegalEntityId));
        }
    }
}
