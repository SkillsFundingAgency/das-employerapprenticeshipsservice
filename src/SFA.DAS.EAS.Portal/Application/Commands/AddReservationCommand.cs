using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.CosmosDb;
using SFA.DAS.EAS.Portal.Database;
using SFA.DAS.EAS.Portal.Database.Models;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.EAS.Portal.Application.Commands
{
    public class AddReservationCommand
    {
        private readonly IAccountsRepository _accountsRepository;
        private readonly ILogger<AddReservationCommand> _logger;

        public AddReservationCommand(IAccountsRepository accountsRepository, ILogger<AddReservationCommand> logger)
        {
            _accountsRepository = accountsRepository;
            _logger = logger;
        }

        public async Task Execute(ReservationCreatedEvent reservedFunding, string messageId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Executing AddReservationCommand");

            var account = await _accountsRepository
                .CreateQuery()
                .SingleOrDefaultAsync(a => a.AccountId == reservedFunding.AccountId, cancellationToken);

            if (account == null)
            {
                //first time we've had *any* event relating to this account
                account = new Account(reservedFunding.AccountId, reservedFunding.AccountLegalEntityId, reservedFunding.AccountLegalEntityName,
                    reservedFunding.Id, reservedFunding.CourseId, reservedFunding.CourseName,
                    reservedFunding.StartDate, reservedFunding.EndDate, reservedFunding.CreatedDate, messageId);

                var newOrg = new Types.Organisation() { Id = reservedFunding.AccountLegalEntityId };
                account.Organisations.Add(newOrg);
                newOrg.Reservations.Add(new Types.Reservation()
                {
                    Id = reservedFunding.Id,
                    CourseCode = reservedFunding.CourseId,
                    CourseName = reservedFunding.CourseName,
                    StartDate = reservedFunding.StartDate,
                    EndDate = reservedFunding.EndDate
                });

                await _accountsRepository.Add(account, null, cancellationToken);
            }
            else
            {
                // account may have been created from non reserved funding event, and there are no reserved funds
                // or this is a new reserved funding, but existing reserved fundings exist
                // or account already contains this reserve funding event (method needs to be idempotent)
                account.AddReserveFunding(reservedFunding.AccountLegalEntityId, reservedFunding.AccountLegalEntityName,
                    reservedFunding.Id, reservedFunding.CourseId, reservedFunding.CourseName,
                    reservedFunding.StartDate, reservedFunding.EndDate, reservedFunding.CreatedDate, messageId);

                var org = account.Organisations.Where(o => o.Id.Equals(reservedFunding.AccountLegalEntityId)).First();

                org.Reservations.Add(new Types.Reservation()
                {
                    Id = reservedFunding.Id,
                    CourseCode = reservedFunding.CourseId,
                    CourseName = reservedFunding.CourseName,
                    StartDate = reservedFunding.StartDate,
                    EndDate = reservedFunding.EndDate
                });

                await _accountsRepository.Update(account, null, cancellationToken);
            }
        }
    }
}
