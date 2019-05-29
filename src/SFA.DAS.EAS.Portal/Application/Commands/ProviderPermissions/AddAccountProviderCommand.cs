using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.Providers.Api.Client;

namespace SFA.DAS.EAS.Portal.Application.Commands.ProviderPermissions
{
    /// <remarks>
    /// Currently, we shouldn't be asked to process an AddedAccountProviderEvent for a provider twice (message is de-duped),
    /// so there shouldn't be an existing provider.
    /// However, as we're allowing message handlers to create a skeleton account document with the details they have,
    /// if a later message is received out-of-order (no message handler currently exists) and has created a provider,
    /// then as this is the canonical event for creating a account/provider relationship, and we fetch the latest provider details anyway,
    /// we update any existing provider with the latest details (rather than throwing or logging a warning).
    /// Note: there is currently no way to remove or delete a provider relationship, so we don't have to worry about that.
    /// </remarks>
    public class AddAccountProviderCommand
    {
        private readonly IAccountDocumentService _accountDocumentService;
        private readonly IProviderApiClient _providerApiClient;
        private readonly ILogger<AddAccountProviderCommand> _logger;

        public AddAccountProviderCommand(
            IAccountDocumentService accountDocumentService,
            IProviderApiClient providerApiClient,
            ILogger<AddAccountProviderCommand> logger)
        {
            _accountDocumentService = accountDocumentService;
            _providerApiClient = providerApiClient;
            _logger = logger;
        }

        public async Task Execute(AddedAccountProviderEvent addedAccountProviderEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Executing {nameof(AddAccountProviderCommand)}");

            var providerTask = _providerApiClient.GetAsync(addedAccountProviderEvent.ProviderUkprn);
            //todo: move getorcreate/ensure into helper. where? service, base, elsewhere?
            var accountDocument = await _accountDocumentService.Get(addedAccountProviderEvent.AccountId, cancellationToken) ??
                                  AccountDocument.Create(addedAccountProviderEvent.AccountId);

            var provider = await providerTask;
            if (provider == null)
                throw new Exception($"Provider with UKPRN={addedAccountProviderEvent.ProviderUkprn} not found");
            
            var accountProvider = accountDocument.Account.Providers.SingleOrDefault(p => p.Ukprn == addedAccountProviderEvent.ProviderUkprn);
            if (accountProvider == null)
            {
                accountProvider = new Provider();
                accountDocument.Account.Providers.Add(accountProvider);
            }

            var primaryAddress = provider.Addresses.SingleOrDefault(a => a.ContactType == "PRIMARY");
            
            accountProvider.Ukprn = addedAccountProviderEvent.ProviderUkprn;
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