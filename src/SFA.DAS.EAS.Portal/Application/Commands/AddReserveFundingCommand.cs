using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.CosmosDb;
using SFA.DAS.EAS.Portal.Database;
using SFA.DAS.EAS.Portal.Database.Models;
using SFA.DAS.EAS.Portal.TempEvents;

namespace SFA.DAS.EAS.Portal.Application.Commands
{
    public class AddReserveFundingCommand
    {
        private readonly IAccountsRepository _accountsRepository;
        //todo: logging
        //private readonly ILogger _logger;

        public AddReserveFundingCommand(IAccountsRepository accountsRepository)//, ILogger logger)
        {
            _accountsRepository = accountsRepository;
            //_logger = logger;
        }

        public async Task Execute(ReserveFundingAddedEvent reservedFunding, string messageId, CancellationToken cancellationToken = default(CancellationToken))
        {
            //_logger.LogInformation("Executing AddReserveFundingCommand");

            // can we have accountid as key, rather than guid??
            var account = await _accountsRepository
                .CreateQuery()
                .SingleOrDefaultAsync(a => a.AccountId == reservedFunding.AccountId, cancellationToken);

            if (account == null)
            {
                //first time we've had *any* event relating to this account
                //todo: do we want to set the message at create time? or more specific add/update
                // will there likely be a message that just creates the account -> employer accounts create account message?
                // need to handle somehow, otherwise add will think message has already been processed
                // we'll remove message id from ctor for now and revisit if needed
                account = new Account(reservedFunding.AccountId, reservedFunding.AccountLegalEntityId, reservedFunding.LegalEntityName,
                    reservedFunding.ReservationId, reservedFunding.CourseId, reservedFunding.CourseName,
                    reservedFunding.StartDate, reservedFunding.EndDate, reservedFunding.Created, messageId);

                await _accountsRepository.Add(account, null, cancellationToken);
            }
            else
            {
                // account may have been created from non reserved funding event, and there are no reserved funds
                // or this is a new reserved funding, but existing reserved fundings exist
                // or account already contains this reserve funding event (method needs to be idempotent)
                account.AddReserveFunding(reservedFunding.AccountLegalEntityId, reservedFunding.LegalEntityName,
                    reservedFunding.ReservationId, reservedFunding.CourseId, reservedFunding.CourseName,
                    reservedFunding.StartDate, reservedFunding.EndDate, reservedFunding.Created, messageId);

                await _accountsRepository.Update(account, null, cancellationToken);
            }
        }
    }
}
