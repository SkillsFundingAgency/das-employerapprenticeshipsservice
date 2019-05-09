using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EAS.Portal.Application.Commands;
using SFA.DAS.EAS.Portal.Events.Reservations;

namespace SFA.DAS.EAS.Portal.Worker.EventHandlers.Reservations
{
    class ReservationCreatedEventHandler : IHandleMessages<ReservationCreatedEvent>
    {
        // NServiceBus can't inject an interface message with methods
        private readonly AddReservationCommand _addReservationCommand;

        public ReservationCreatedEventHandler(AddReservationCommand addReservationCommand)
        {
            _addReservationCommand = addReservationCommand;
        }

        public Task Handle(ReservationCreatedEvent message, IMessageHandlerContext context)
        {
            return _addReservationCommand.Execute(message, context.MessageId);
        }
    }
}
