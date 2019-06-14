using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.Portal.Application.EventHandlers;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.EAS.Portal.Worker.EventHandlers.Reservations
{
    public class ReservationCreatedEventHandler : EventHandler<ReservationCreatedEvent>
    {
        private readonly IEventHandler<ReservationCreatedEvent> _handler;

        public ReservationCreatedEventHandler(
            IEventHandler<ReservationCreatedEvent> handler,
            IMessageContextInitialisation messageContextInitialisation)
                : base(messageContextInitialisation)
        {
            _handler = handler;
        }

        protected override Task Handle(ReservationCreatedEvent reservationCreatedEvent, CancellationToken cancellationToken = default)
        {
            return _handler.Handle(reservationCreatedEvent, cancellationToken);
        }
    }
}
