using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.EAS.Portal.Application.Commands.Reservation
{
    public class AddReservationCommand : Command, ICommand<ReservationCreatedEvent>
    {
        private readonly ILogger<AddReservationCommand> _logger;

        public AddReservationCommand(
            IAccountDocumentService accountDocumentService,
            ILogger<AddReservationCommand> logger)
        : base(accountDocumentService)
        {
            _logger = logger;
        }

        public async Task Execute(ReservationCreatedEvent reservationCreatedEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Executing {nameof(AddReservationCommand)}");

            var accountDocument = await GetOrCreateAccountDocument(reservationCreatedEvent.AccountId, cancellationToken);

            var (organisation, organisationCreation) = GetOrAddOrganisation(accountDocument, reservationCreatedEvent.AccountLegalEntityId);
            if (organisationCreation == EntityCreation.Created)
            {
                organisation.Name = reservationCreatedEvent.AccountLegalEntityName;
            }
            else
            {
                var existingReservation = organisation.Reservations.FirstOrDefault(r => r.Id.Equals(reservationCreatedEvent.Id));
                if (existingReservation != null)
                    throw DuplicateReservationCreatedEventException(reservationCreatedEvent);
            }
            
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
