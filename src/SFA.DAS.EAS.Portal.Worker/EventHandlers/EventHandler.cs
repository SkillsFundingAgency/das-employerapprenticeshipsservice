using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EAS.Portal.Application.Services;

namespace SFA.DAS.EAS.Portal.Worker.EventHandlers
{
    public abstract class EventHandler<TEvent> : IHandleMessages<TEvent>
    {
        private readonly IMessageContextInitialisation _messageContextInitialisation;

        protected EventHandler(IMessageContextInitialisation messageContextInitialisation)
        {
            _messageContextInitialisation = messageContextInitialisation;
        }
        
        public Task Handle(TEvent message, IMessageHandlerContext context)
        {   
            _messageContextInitialisation.Initialise(context);
            return Handle(message, new CancellationToken());
        }

        protected abstract Task Handle(TEvent message, CancellationToken cancellationToken = default);
    }
}