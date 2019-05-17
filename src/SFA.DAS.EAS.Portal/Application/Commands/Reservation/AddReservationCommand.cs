using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Database.Models;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.EAS.Portal.Application.Commands.Reservation
{
    public class AddReservationCommand
    {
        private readonly IAccountDocumentService _accountsService;
        private readonly ILogger<AddReservationCommand> _logger;

        public AddReservationCommand(IAccountDocumentService accountsService, ILogger<AddReservationCommand> logger)
        {
            _accountsService = accountsService;
            _logger = logger;
        }

        public async Task Execute(ReservationCreatedEvent reservedFunding, string messageId, CancellationToken cancellationToken = default)
        {
            // TODO: move logging to a decorator
            _logger.LogInformation("Executing AddReservationCommand");

            var accountDocument = await _accountsService.Get(reservedFunding.AccountId);

            if (accountDocument == null)
            {
                accountDocument = AccountDocument.Create(reservedFunding.AccountId);
                accountDocument.Account.Name = reservedFunding.AccountLegalEntityName;

                var newOrg = new Types.Organisation() {
                    Id = reservedFunding.AccountLegalEntityId,
                    Name = reservedFunding.AccountLegalEntityName };

                accountDocument.Account.Organisations.Add(newOrg);

                newOrg.Reservations.Add(new Types.Reservation()
                {
                    Id = reservedFunding.Id,
                    CourseCode = reservedFunding.CourseId,
                    CourseName = reservedFunding.CourseName,
                    StartDate = reservedFunding.StartDate,
                    EndDate = reservedFunding.EndDate
                });
            }
            else
            {
                var org = accountDocument.Account.Organisations.Where(o => o.Id.Equals(reservedFunding.AccountLegalEntityId)).First();

                var existing = org.Reservations.FirstOrDefault(r => r.Id.Equals(reservedFunding.Id));
                if (existing != null)
                {
                    return;  // already handled 
                }

                org.Reservations.Add(new Types.Reservation()
                {
                    Id = reservedFunding.Id,
                    CourseCode = reservedFunding.CourseId,
                    CourseName = reservedFunding.CourseName,
                    StartDate = reservedFunding.StartDate,
                    EndDate = reservedFunding.EndDate
                });
            }

            await _accountsService.Save(accountDocument);
        }
    }
}
