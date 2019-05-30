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

        protected enum EntityCreation
        {
            Created,
            Existed
        }
        
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

//        protected Organisation GetOrAddOrganisation(AccountDocument accountDocument, long accountLegalEntityId, Action<Organisation> populateOnCreate = null)
//        {
//            var organisation = accountDocument.Account.Organisations.SingleOrDefault(o => o.AccountLegalEntityId == accountLegalEntityId);
//            if (organisation == null)
//            {
//                organisation = new Organisation {AccountLegalEntityId = accountLegalEntityId};
//                populateOnCreate?.Invoke(organisation);
//                accountDocument.Account.Organisations.Add(organisation);
//            }
//
//            return organisation;
//        }

        protected (Organisation, EntityCreation) GetOrAddOrganisation(AccountDocument accountDocument, long accountLegalEntityId)
        {
            var organisation = accountDocument.Account.Organisations.SingleOrDefault(o => o.AccountLegalEntityId == accountLegalEntityId);
            if (organisation == null)
            {
                organisation = new Organisation {AccountLegalEntityId = accountLegalEntityId};
                accountDocument.Account.Organisations.Add(organisation);
                return (organisation, EntityCreation.Created);
            }

            return (organisation, EntityCreation.Existed);
        }

        protected (Provider, EntityCreation) GetOrAddProvider(AccountDocument accountDocument, long ukprn)
        {
            var provider = accountDocument.Account.Providers.SingleOrDefault(p => p.Ukprn == ukprn);
            if (provider == null)
            {
                provider = new Provider {Ukprn = ukprn};
                accountDocument.Account.Providers.Add(provider);
                return (provider, EntityCreation.Created);
            }

            return (provider, EntityCreation.Existed);
        }
    }
}