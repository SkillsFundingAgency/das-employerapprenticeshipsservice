using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.EAS.Portal.Client.Types;

namespace SFA.DAS.EAS.Portal.Application.Commands
{
    public class Command
    {
        private readonly IAccountDocumentService _accountDocumentService;

        protected Command(IAccountDocumentService accountDocumentService)
        //todo: : IPortalCommand<TEvent>
        {
            _accountDocumentService = accountDocumentService;
        }
        
        protected async Task<AccountDocument> GetOrCreateAccountDocument(long accountId, CancellationToken cancellationToken = default)
        {
            return await _accountDocumentService.Get(accountId, cancellationToken) ??
                AccountDocument.Create(accountId);
        }

        // these could be extensions, but not good for testing
        protected Provider GetOrAddProvider(AccountDocument accountDocument, long ukprn)
        {
            var provider = accountDocument.Account.Providers.SingleOrDefault(p => p.Ukprn == ukprn);
            if (provider == null)
            {
                provider = new Provider();
                accountDocument.Account.Providers.Add(provider);
            }

            return provider;
        }
    }
}