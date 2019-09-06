using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Portal.Application.Services.AccountDocumentService;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.EAS.Portal.TypesExtensions;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.EAS.Portal.Application.EventHandlers.Reservations
{
    public class ReservationCreatedEventHandler : IEventHandler<ReservationCreatedEvent>
    {
        private readonly IAccountDocumentService _accountDocumentService;
        private readonly ILogger<ReservationCreatedEventHandler> _logger;

        public ReservationCreatedEventHandler(
            IAccountDocumentService accountDocumentService,
            ILogger<ReservationCreatedEventHandler> logger)
        {
            _accountDocumentService = accountDocumentService;
            _logger = logger;
        }

        public async Task Handle(ReservationCreatedEvent reservationCreatedEvent, CancellationToken cancellationToken = default)
        {
            var accountDocument = await _accountDocumentService.GetOrCreate(reservationCreatedEvent.AccountId, cancellationToken);

            var organisation = accountDocument.Account.GetOrAddOrganisation(reservationCreatedEvent.AccountLegalEntityId,
                addedOrganisation => addedOrganisation.Name = reservationCreatedEvent.AccountLegalEntityName,
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