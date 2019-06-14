using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.Portal.Application.EventHandlers;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.ProviderRelationships.Messages.Events;

namespace SFA.DAS.EAS.Portal.Worker.EventHandlers.ProviderRelationships
{
    public class AddedAccountProviderEventHandler : EventHandler<AddedAccountProviderEvent>
    {
        private readonly IEventHandler<AddedAccountProviderEvent> _handler;

        public AddedAccountProviderEventHandler(
            IEventHandler<AddedAccountProviderEvent> handler,
            IMessageContextInitialisation messageContextInitialisation)
                : base(messageContextInitialisation)
        {
            _handler = handler;
        }

        protected override Task Handle(AddedAccountProviderEvent addedAccountProviderEvent, CancellationToken cancellationToken = default)
        {
            return _handler.Handle(addedAccountProviderEvent, cancellationToken);
        }
    }
}