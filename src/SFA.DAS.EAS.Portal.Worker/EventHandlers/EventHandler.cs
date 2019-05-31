using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EAS.Portal.Application.Commands;
using SFA.DAS.EAS.Portal.Application.Services;

namespace SFA.DAS.EAS.Portal.Worker.EventHandlers
{
    public class EventHandler<TEvent> : IHandleMessages<TEvent>
    {
        private readonly ICommand<TEvent> _command;
        private readonly IMessageContext _messageContext;

        protected EventHandler(ICommand<TEvent> command, IMessageContext messageContext)
        {
            _command = command;
            _messageContext = messageContext;
        }
        
        public Task Handle(TEvent message, IMessageHandlerContext context)
        {
            _messageContext.Initialise(context);
            return _command.Execute(message);
        }
    }
}