using SFA.DAS.CosmosDb;
using SFA.DAS.EAS.Portal.Database;
using SFA.DAS.EAS.Portal.Database.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Portal.Application.Services
{
    public class AccountsService : IAccountsService
    {
        private readonly IAccountsRepository _accountsRepository;
        public AccountsService(IAccountsRepository accountsRepository)
        {
            _accountsRepository = accountsRepository;
        }
        public Task<Account> Get(long id, CancellationToken cancellationToken = default)
        {
            return _accountsRepository
               .CreateQuery()
               .SingleOrDefaultAsync(a => a.AccountId == id, cancellationToken);
        }

        public async Task Save(string messageId, Account account, CancellationToken cancellationToken = default)
        {
            account.DeleteOldMessages();
            if (account.IsMessageProcessed(messageId)){ return; };

            await _accountsRepository.Update(account, null, cancellationToken);

            account.AddOutboxMessage(messageId, DateTime.Now);
        }
    }
}
