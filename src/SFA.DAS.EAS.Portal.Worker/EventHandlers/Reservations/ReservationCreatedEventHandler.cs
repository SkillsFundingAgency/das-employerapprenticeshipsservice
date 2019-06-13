using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.EAS.Portal.Worker.TypesExtensions;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.EAS.Portal.Worker.EventHandlers.Reservations
{
    public class ReservationCreatedEventHandler : EventHandler<ReservationCreatedEvent>
    {
        public ReservationCreatedEventHandler(
            IAccountDocumentService accountDocumentService,
            IMessageContextInitialisation messageContextInitialisation,
            ILogger<ReservationCreatedEventHandler> logger)
                : base(accountDocumentService, messageContextInitialisation, logger)
        {
        }

        protected override async Task Handle(ReservationCreatedEvent reservationCreatedEvent)
        {
            var cancellationToken = default(CancellationToken);

            var accountDocument = await GetOrCreateAccountDocument(reservationCreatedEvent.AccountId, cancellationToken);

            var organisation = accountDocument.Account.GetOrAddOrganisation(reservationCreatedEvent.AccountLegalEntityId,
                addedOrganisation =>
                {
                    addedOrganisation.Name = reservationCreatedEvent.AccountLegalEntityName;
                },
                existingOrganisation =>
                {
                    var existingReservation = existingOrganisation.Reservations.FirstOrDefault(r => r.Id.Equals(reservationCreatedEvent.Id));
                    if (existingReservation != null)
                        throw DuplicateReservationCreatedEventException(reservationCreatedEvent);
                });
            
            organisation.Reservations.Add(new Client.Types.Reservation
            {
                Id = reservationCreatedEvent.Id,
                CourseCode = reservationCreatedEvent.CourseId,
                CourseName = reservationCreatedEvent.CourseName,
                StartDate = reservationCreatedEvent.StartDate,
                EndDate = reservationCreatedEvent.EndDate
            });
            
            await AccountDocumentService.Save(accountDocument, cancellationToken);
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
