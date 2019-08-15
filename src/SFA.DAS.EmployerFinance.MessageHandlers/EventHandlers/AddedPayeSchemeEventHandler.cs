using System.Threading.Tasks;
using MediatR;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerFinance.Commands.CreateAccountPaye;
using SFA.DAS.EmployerFinance.Messages.Commands;

namespace SFA.DAS.EmployerFinance.MessageHandlers.EventHandlers
{
    public class AddedPayeSchemeEventHandler : IHandleMessages<AddedPayeSchemeEvent>
    {
        private readonly IMediator _mediator;

        public AddedPayeSchemeEventHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(AddedPayeSchemeEvent message, IMessageHandlerContext context)
        {
            await _mediator.SendAsync(new CreateAccountPayeCommand(message.AccountId, message.PayeRef,message.SchemeName, message.Aorn));
        }
    }
}
