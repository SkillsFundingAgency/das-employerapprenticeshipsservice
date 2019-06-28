using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Portal.Application.EventHandlers;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.ProviderRelationships.Messages.Events;

namespace SFA.DAS.EAS.Portal.Worker.EventHandlers.ProviderRelationships
{
    public class AddedAccountProviderEventHandler : EventHandler<AddedAccountProviderEvent>
    {
        public AddedAccountProviderEventHandler(
            IMessageContextInitialisation messageContextInitialisation,
            IEventHandler<AddedAccountProviderEvent> reservationCreatedEventHandler,
            ILogger<AddedAccountProviderEventHandler> logger)
            : base(messageContextInitialisation, reservationCreatedEventHandler, logger)
        {
        }
    }
}