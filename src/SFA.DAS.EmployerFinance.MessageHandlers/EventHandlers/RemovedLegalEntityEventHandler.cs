using System.Threading.Tasks;
using MediatR;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerFinance.Commands.RemoveAccountLegalEntity;

namespace SFA.DAS.EmployerFinance.MessageHandlers.EventHandlers
{
    public class RemovedLegalEntityEventHandler : IHandleMessages<RemovedLegalEntityEvent>
    {
        private readonly IMediator _mediator;

        public RemovedLegalEntityEventHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task Handle(RemovedLegalEntityEvent message, IMessageHandlerContext context)
        {
            return _mediator.SendAsync(new RemoveAccountLegalEntityCommand(message.AccountLegalEntityId));
        }
    }
}