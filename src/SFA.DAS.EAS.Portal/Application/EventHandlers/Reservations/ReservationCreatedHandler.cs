using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.Reservations.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.Portal.Extensions;
using System.Linq;
using SFA.DAS.EAS.Portal.Client.Database.Models;

namespace SFA.DAS.EAS.Portal.Application.EventHandlers.Reservations
{
    public class ReservationCreatedHandler : IEventHandler<ReservationCreatedEvent>
    {
        private readonly IAccountDocumentService _accountDocumentService;

        public ReservationCreatedHandler(IAccountDocumentService accountDocumentService)
        {
            _accountDocumentService = accountDocumentService;
        }

        public async Task Handle(ReservationCreatedEvent @event, CancellationToken cancellationToken = default)
        {
            var accountDocument = await _accountDocumentService.GetOrCreate(@event.AccountId, cancellationToken);

            var organisation = accountDocument.Account.GetOrAddOrganisation(@event.AccountLegalEntityId,
                addedOrganisation =>
                {
                    addedOrganisation.Name = @event.AccountLegalEntityName;
                },
                existingOrganisation =>
                {
                    var existingReservation = existingOrganisation.Reservations.FirstOrDefault(r => r.Id.Equals(@event.Id));
                    if (existingReservation != null)
                        throw DuplicateReservationCreatedEventException(@event);
                });

            organisation.Reservations.Add(new Client.Types.Reservation
            {
                Id = @event.Id,
                CourseCode = @event.CourseId,
                CourseName = @event.CourseName,
                StartDate = @event.StartDate,
                EndDate = @event.EndDate
            });

            await _accountDocumentService.Save(accountDocument, cancellationToken);
        }

        private Exception DuplicateReservationCreatedEventException(ReservationCreatedEvent reservationCreatedEvent)
        {
            return new Exception(
                $@"Received {nameof(ReservationCreatedEvent)} with 
                    Id:{reservationCreatedEvent.Id}, 
                    AccountId:{reservationCreatedEvent.AccountId},
                    AccountLegalEntityId:{reservationCreatedEvent.AccountLegalEntityId},
                    AccountLegalEntityName:{reservationCreatedEvent.AccountLegalEntityName},
                    CourseId:{reservationCreatedEvent.CourseId},
                    CourseName:{reservationCreatedEvent.CourseName},
                    StartDate:{reservationCreatedEvent.StartDate},
                    EndDate:{reservationCreatedEvent.EndDate},
                    when {nameof(AccountDocument)} already contains a reservation with that Id.");
        }
    }
}
