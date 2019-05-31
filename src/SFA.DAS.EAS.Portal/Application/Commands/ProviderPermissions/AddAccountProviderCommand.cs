using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.Providers.Api.Client;

namespace SFA.DAS.EAS.Portal.Application.Commands.ProviderPermissions
{
    //todo: if we have time this sprint, subscribe to provider updated event (is there 1?) and update our local store provider details
    // if not, add tech-debt item to backlog

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
    public class AddAccountProviderCommand : Command, IPortalCommand<AddedAccountProviderEvent>
    {
        private readonly IAccountDocumentService _accountDocumentService;
        private readonly IProviderApiClient _providerApiClient;
        private readonly ILogger<AddAccountProviderCommand> _logger;

        //todo: optimisation: these commands should be able to be singleton
        public AddAccountProviderCommand(
            IAccountDocumentService accountDocumentService,
            IProviderApiClient providerApiClient,
            ILogger<AddAccountProviderCommand> logger)
        : base(accountDocumentService)
        {
            _accountDocumentService = accountDocumentService;
            _providerApiClient = providerApiClient;
            _logger = logger;
        }

        public async Task Execute(AddedAccountProviderEvent addedAccountProviderEvent, CancellationToken cancellationToken = default)
        {
            //todo: cross cutting logging. handler could log/decorator/other?
            _logger.LogInformation($"Executing {nameof(AddAccountProviderCommand)}");

            var providerTask = _providerApiClient.GetAsync(addedAccountProviderEvent.ProviderUkprn);
            var accountDocument = await GetOrCreateAccountDocument(addedAccountProviderEvent.AccountId, cancellationToken);
            
            var (accountProvider,_) = GetOrAddProvider(accountDocument, addedAccountProviderEvent.ProviderUkprn);

            var provider = await providerTask;
            var primaryAddress = provider.Addresses.SingleOrDefault(a => a.ContactType == "PRIMARY");
            //todo: we could fallback to LEGAL address if PRIMARY not found
            
            accountProvider.Name = provider.ProviderName;
            accountProvider.Email = provider.Email;
            accountProvider.Phone = provider.Phone;
            accountProvider.Street = primaryAddress?.Street;
            accountProvider.Town = primaryAddress?.Town;
            accountProvider.Postcode = primaryAddress?.PostCode;
                
            await _accountDocumentService.Save(accountDocument, cancellationToken);
        }
    }
}