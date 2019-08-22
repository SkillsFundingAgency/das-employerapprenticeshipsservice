using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.EAS.Portal.Application.EventHandlers;
using SFA.DAS.EAS.Portal.Application.Services.MessageContext;

namespace SFA.DAS.EAS.Portal.Worker.EventHandlers
{
    public class EventHandler<TEvent> : IHandleMessages<TEvent>
    {
        //todo: better name
        protected readonly IEventHandler<TEvent> EventHandlerPartTwo;
        protected readonly ILogger Logger;
        private readonly IMessageContextInitialisation _messageContextInitialisation;

        protected EventHandler(
            IMessageContextInitialisation messageContextInitialisation,
            IEventHandler<TEvent> eventHandler,
            ILogger logger)
        {
            _messageContextInitialisation = messageContextInitialisation;
            EventHandlerPartTwo = eventHandler;
            Logger = logger;
        }
        
        public Task Handle(TEvent message, IMessageHandlerContext context)
        {
            _messageContextInitialisation.Initialise(context);
            return EventHandlerPartTwo.Handle(message);
        }
    }
}