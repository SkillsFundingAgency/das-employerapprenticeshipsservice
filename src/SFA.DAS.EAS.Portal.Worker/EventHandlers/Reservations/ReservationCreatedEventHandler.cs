using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Portal.Application.EventHandlers;
using SFA.DAS.EAS.Portal.Application.Services.MessageContext;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.EAS.Portal.Worker.EventHandlers.Reservations
{
    public class ReservationCreatedEventHandler : EventHandler<ReservationCreatedEvent>
    {
        public ReservationCreatedEventHandler(
            IMessageContextInitialisation messageContextInitialisation,
            IEventHandler<ReservationCreatedEvent> reservationCreatedEventHandler,
            ILogger<ReservationCreatedEventHandler> logger)
                : base(messageContextInitialisation, reservationCreatedEventHandler, logger)
        {
        }
    }
}
