using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EAS.Portal.Application.Commands.Reservation;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Worker.Extensions;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.EAS.Portal.Worker.EventHandlers.Reservations
{
    class ReservationCreatedEventHandler : IHandleMessages<ReservationCreatedEvent>
    {
        private readonly IMessageContext _messageContext;
        private readonly AddReservationCommand _addReservationCommand;

        public ReservationCreatedEventHandler(AddReservationCommand addReservationCommand, IMessageContext messageContext)
        {
            _addReservationCommand = addReservationCommand;
            _messageContext = messageContext;
        }

        public Task Handle(ReservationCreatedEvent message, IMessageHandlerContext context)
        {
            _messageContext.Initialise(context);
            return _addReservationCommand.Execute(message);
        }
    }
}
