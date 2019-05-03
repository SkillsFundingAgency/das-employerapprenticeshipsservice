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
        //todo: reinstate logging
        //private readonly ILogger _logger;

        //public AddReserveFundingCommand(ILogger logger)
        //{
        //    _logger = logger;
        //}

        //todo: add service
        public AddReserveFundingCommand(IAccountsRepository accountsRepository)
        {
            _accountsRepository = accountsRepository;
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
                account = new Account(reservedFunding.AccountId, reservedFunding.Created, messageId);

                //todo: create with reserve funding ctor, or new then add??
                //account.AddReserveFunding(gubbins, eventCreated, messageId);

                //todo: rest of gubbins
                await _accountsRepository.Add(account, null, cancellationToken);
            }
            else
            {
                // account may have been created from non reserved funding event, and there are no reserved funds
                // or this is a new reserved funding, but existing reserved fundings exist
                // or account already contains this reserve funding event (method needs to be idempotent)
                //todo:
                //account.AddReserveFunding(gubbins, eventCreated, messageId);

                await _accountsRepository.Update(account, null, cancellationToken);
            }
        }
    }
}
