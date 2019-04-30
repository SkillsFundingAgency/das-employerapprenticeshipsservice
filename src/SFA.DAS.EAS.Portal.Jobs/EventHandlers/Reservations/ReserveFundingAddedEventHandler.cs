using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EAS.Portal.Application.Commands;

namespace SFA.DAS.EAS.Portal.Jobs.EventHandlers.Reservations
{
    public class ReserveFundingAddedEvent // : Event
    {
        public long AccountId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string LegalEntityName { get; set; }
        public long CourseId { get; set; }
        public string CourseName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime Created { get; set; }
    }

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
