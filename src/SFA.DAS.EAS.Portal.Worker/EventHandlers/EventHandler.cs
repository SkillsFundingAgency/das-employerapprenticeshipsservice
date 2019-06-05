using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EAS.Portal.Application.Commands;
using SFA.DAS.EAS.Portal.Application.Services;

namespace SFA.DAS.EAS.Portal.Worker.EventHandlers
{
    public class EventHandler<TEvent> : IHandleMessages<TEvent>
    {
        private readonly ICommand<TEvent> _command;
        private readonly IMessageContextInitialisation _messageContextInitialisation;

        protected EventHandler(ICommand<TEvent> command, IMessageContextInitialisation messageContextInitialisation)
        {
            _command = command;
            _messageContextInitialisation = messageContextInitialisation;
        }
        
        public Task Handle(TEvent message, IMessageHandlerContext context)
        {
            _messageContextInitialisation.Initialise(context);
            return _command.Execute(message);
        }
    }
}