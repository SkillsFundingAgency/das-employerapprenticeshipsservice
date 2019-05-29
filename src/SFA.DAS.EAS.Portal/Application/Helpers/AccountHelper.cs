using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Portal.Application.AccountHelper
{
    public class AccountHelperService : IAccountHelperService
    {
        private readonly IAccountDocumentService _accountsService;

        public AccountHelperService(
            IAccountDocumentService accountsService)
        {
            _accountsService = accountsService;
        }

        public async Task<AccountDocument> GetOrCreateAccount(long accountId, CancellationToken cancellationToken = default)
        {
            var accountDocument = await _accountsService.Get(accountId, cancellationToken);
            if(accountDocument == null)
            {
                accountDocument = AccountDocument.Create(accountId);
            }
            return accountDocument;
        }

    }
}
