using SFA.DAS.CosmosDb;
using SFA.DAS.EAS.Portal.Database;
using SFA.DAS.EAS.Portal.Database.Models;
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

        public Task Save(Account account, CancellationToken cancellationToken = default)
        {
            return _accountsRepository.Update(account, null, cancellationToken);
        }
    }
}
