using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Portal.Application.Services;
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

        public async Task Execute(ReservationCreatedEvent reservedFunding, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Executing {nameof(AddReservationCommand)}");

            var accountDocument = await GetOrCreateAccountDocument(reservedFunding.AccountId, cancellationToken);

            var (organisation, organisationCreation) = GetOrAddOrganisation(accountDocument, reservedFunding.AccountLegalEntityId);
            if (organisationCreation == EntityCreation.Created)
            {
                organisation.Name = reservedFunding.AccountLegalEntityName;
            }
            else
            {
                var existingReservation = organisation.Reservations.FirstOrDefault(r => r.Id.Equals(reservedFunding.Id));
                if (existingReservation != null)
                {
                    //todo: change to throw
                    _logger.LogInformation($"ReservationCreatedEvent received for a reservation (Id: {reservedFunding.Id}) that has already been handled. The event will be ignored.");
                    return;  // already handled 
                }
            }
            
            organisation.Reservations.Add(new Client.Types.Reservation
            {
                Id = reservedFunding.Id,
                CourseCode = reservedFunding.CourseId,
                CourseName = reservedFunding.CourseName,
                StartDate = reservedFunding.StartDate,
                EndDate = reservedFunding.EndDate
            });
            
            await AccountDocumentService.Save(accountDocument, cancellationToken);
        }
    }
}
