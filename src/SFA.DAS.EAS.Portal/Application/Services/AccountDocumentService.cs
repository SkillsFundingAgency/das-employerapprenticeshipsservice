using SFA.DAS.CosmosDb;
using SFA.DAS.EAS.Portal.Database;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Portal.Application.Services
{
    public class AccountDocumentService : IAccountDocumentService
    {
        private readonly IAccountsRepository _accountsRepository;
        
        public AccountDocumentService(IAccountsRepository accountsRepository)
        {
            _accountsRepository = accountsRepository;
        }
        public async Task<AccountDocument> GetOrCreate(long id, CancellationToken cancellationToken = default)
        {
            var accountDocument = await _accountsRepository.CreateQuery()
               .SingleOrDefaultAsync(a => a.AccountId == id, cancellationToken);
            if (accountDocument == null)
            {
                accountDocument = AccountDocument.Create(id);
            }
            return accountDocument;
        }

        public Task Save(AccountDocument account, CancellationToken cancellationToken = default)
        {
            if (account.IsNew)
            {               
                return _accountsRepository.Add(account, null, cancellationToken);
            }

            return _accountsRepository.Update(account, null, cancellationToken);
        }
    }
}
