using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Portal.Application.Commands.Account;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.EAS.Portal.Application.Commands.Reservation
{
    public class AddReservationCommand
    {
        private readonly IAccountDocumentService _accountDocumentService;
        private readonly ICommandHandler<AccountCreatedCommand> _accountCreatedHandler;
        private readonly ILogger<AddReservationCommand> _logger;

        public AddReservationCommand(
            IAccountDocumentService accountDocumentService,
            ICommandHandler<AccountCreatedCommand> accountCreatedHandler,
            ILogger<AddReservationCommand> logger)
        {
            _accountDocumentService = accountDocumentService;
            _accountCreatedHandler = accountCreatedHandler;
            _logger = logger;
        }

        public async Task Execute(ReservationCreatedEvent reservedFunding, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Executing AddReservationCommand");

            await _accountCreatedHandler.Handle(new AccountCreatedCommand(reservedFunding.AccountId, reservedFunding.AccountLegalEntityName), cancellationToken);

            var accountDocument = await _accountDocumentService.Get(reservedFunding.AccountId, cancellationToken);

            var org = accountDocument.Account.Organisations.FirstOrDefault(o => o.AccountLegalEntityId.Equals(reservedFunding.AccountLegalEntityId));
            if (org == null)
            {
                CreateOrganisationWithReservation(accountDocument, reservedFunding);
            }
            else
            {
                UpdateOrganisationWithReservation(org, reservedFunding);
            }

            await _accountDocumentService.Save(accountDocument, cancellationToken);
        }

        private void UpdateOrganisationWithReservation(Organisation organisation, ReservationCreatedEvent reservedFunding)
        {
            var existing = organisation.Reservations.FirstOrDefault(r => r.Id.Equals(reservedFunding.Id));
            if (existing != null)
            {
                //todo: change to throw
                _logger.LogInformation($"ReservationCreatedEvent received for a reservation (Id: {reservedFunding.Id}) that has already been handled.  The event will be ignored.");
                return;  // already handled 
            }

            organisation.Reservations.Add(new Client.Types.Reservation()
            {
                Id = reservedFunding.Id,
                CourseCode = reservedFunding.CourseId,
                CourseName = reservedFunding.CourseName,
                StartDate = reservedFunding.StartDate,
                EndDate = reservedFunding.EndDate
            });
        }

        private static void CreateOrganisationWithReservation(AccountDocument accountDocument, ReservationCreatedEvent reservedFunding)
        {
            var newOrg = new Organisation
            {
                AccountLegalEntityId = reservedFunding.AccountLegalEntityId,
                Name = reservedFunding.AccountLegalEntityName
            };

            accountDocument.Account.Organisations.Add(newOrg);

            newOrg.Reservations.Add(new Client.Types.Reservation
            {
                Id = reservedFunding.Id,
                CourseCode = reservedFunding.CourseId,
                CourseName = reservedFunding.CourseName,
                StartDate = reservedFunding.StartDate,
                EndDate = reservedFunding.EndDate
            });
        }
    }
}
