using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EAS.Portal.Application.Commands;
using SFA.DAS.EAS.Portal.Application.Commands.Paye;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Worker.Extensions;
using SFA.DAS.EmployerAccounts.Messages.Events;

namespace SFA.DAS.EAS.Portal.Worker.EventHandlers.PayeScheme
{
    public class AddedPayeSchemeEventHandler : IHandleMessages<AddedPayeSchemeEvent>
    {
        private readonly ICommandHandler<PayeSchemeAddedCommand> _handler;
        private readonly IMessageContext _messageContext;

        public AddedPayeSchemeEventHandler(ICommandHandler<PayeSchemeAddedCommand> handler, IMessageContext messageContext)
        {
            _handler = handler;
            _messageContext = messageContext;
        }

        public async Task Handle(AddedPayeSchemeEvent message, IMessageHandlerContext context)
        {
            _messageContext.Initialise(context);
            await _handler.Handle(new PayeSchemeAddedCommand(
                message.AccountId,
                message.UserName, 
                message.UserRef,
                message.PayeRef,
                message.Created));
        }
    }
}
