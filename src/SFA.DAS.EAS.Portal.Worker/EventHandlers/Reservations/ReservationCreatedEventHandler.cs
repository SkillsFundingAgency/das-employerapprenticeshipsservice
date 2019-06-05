using SFA.DAS.EAS.Portal.Application.Commands;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.EAS.Portal.Worker.EventHandlers.Reservations
{
    public class ReservationCreatedEventHandler : EventHandler<ReservationCreatedEvent>
    {
        public ReservationCreatedEventHandler(ICommand<ReservationCreatedEvent> addReservationCommand, IMessageContextInitialisation messageContextInitialisation)
            : base(addReservationCommand, messageContextInitialisation)
        {
        }
    }
}
