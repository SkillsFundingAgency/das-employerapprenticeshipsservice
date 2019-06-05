using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.Providers.Api.Client;

namespace SFA.DAS.EAS.Portal.Worker.EventHandlers.ProviderRelationships
{
    //todo: subscribe to provider updated event (is there 1?) and update our local store provider details
    
    /// <remarks>
    /// Options for retrieving provider details
    /// ---------------------------------------
    /// a) fetch each individual provider when processing each event [gone for this option]
    /// b) bulk fetch all new overnight and keep in local store
    ///
    /// a) will need to update provider details on provider details changed event
    ///    +ve no web job, less storage
    ///    -ve event handling will fail if provider api down (or we don't store provider details)
    /// b) +ve able to handle message even if provider api down, webjob could update stale details
    ///    -ve more data in store, more infrastructure/complexity
    ///
    /// Notes on handling provider already exists
    /// -----------------------------------------
    /// Currently, we shouldn't be asked to process an AddedAccountProviderEvent for a provider twice (message is de-duped),
    /// so there shouldn't be an existing provider.
    /// However, as we're allowing message handlers to create a skeleton account document with the details they have,
    /// if a later message is received out-of-order (no message handler currently exists) and has created a provider,
    /// then as this is the canonical event for creating a account/provider relationship, and we fetch the latest provider details anyway,
    /// we update any existing provider with the latest details (rather than throwing or logging a warning).
    /// Note: there is currently no way to remove or delete a provider relationship, so we don't have to worry about that.
    /// </remarks>
    public class AddedAccountProviderEventHandler : EventHandler<AddedAccountProviderEvent>
    {
        private readonly IProviderApiClient _providerApiClient;

        public AddedAccountProviderEventHandler(
            IAccountDocumentService accountDocumentService,
            IMessageContextInitialisation messageContextInitialisation,
            //todo: can nservicebus inject logger?
            //ILogger<AddedAccountProviderEventHandler> logger,
            IProviderApiClient providerApiClient)
        //add logger
            : base(accountDocumentService, messageContextInitialisation)
        {
            _providerApiClient = providerApiClient;
        }

        protected override async Task Handle(AddedAccountProviderEvent addedAccountProviderEvent)
        {
            //todo: as we don't get one passed to the handler, and we don't need one, remove our cancellationtoken support?
            var cancellationToken = default(CancellationToken);
            
            var providerTask = _providerApiClient.GetAsync(addedAccountProviderEvent.ProviderUkprn);
            var accountDocument = await GetOrCreateAccountDocument(addedAccountProviderEvent.AccountId, cancellationToken);
            
            var (accountProvider,_) = GetOrAddProvider(accountDocument, addedAccountProviderEvent.ProviderUkprn);

            var provider = await providerTask;
            var address = provider.Addresses.FirstOrDefault(a => a.ContactType == "PRIMARY")
                          ?? provider.Addresses.FirstOrDefault(a => a.ContactType == "LEGAL");
            
            accountProvider.Name = provider.ProviderName;
            accountProvider.Email = provider.Email;
            accountProvider.Phone = provider.Phone;
            accountProvider.Street = address?.Street;
            accountProvider.Town = address?.Town;
            accountProvider.Postcode = address?.PostCode;
                
            await AccountDocumentService.Save(accountDocument, cancellationToken);
        }
    }
}