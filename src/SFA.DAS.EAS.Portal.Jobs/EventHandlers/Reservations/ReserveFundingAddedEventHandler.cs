using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EAS.Portal.Application.Commands;
using SFA.DAS.EAS.Portal.TempEvents;

namespace SFA.DAS.EAS.Portal.Jobs.EventHandlers.Reservations
{
    //todo: rename when know real event name
    class ReserveFundingAddedEventHandler : IHandleMessages<ReserveFundingAddedEvent>
    {
        private readonly AddReserveFundingCommand _addReserveFundingCommand;

        public ReserveFundingAddedEventHandler(AddReserveFundingCommand addReserveFundingCommand)
        {
            _addReserveFundingCommand = addReserveFundingCommand;
        }

        public Task Handle(ReserveFundingAddedEvent message, IMessageHandlerContext context)
        {
            return _addReserveFundingCommand.Execute(message.AccountId, message.AccountLegalEntityId,
                message.LegalEntityName, message.CourseId, message.CourseName, message.StartDate, message.EndDate, message.Created);
        }
    }
}
