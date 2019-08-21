using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Portal.Application.Services.AccountDocumentService;
using SFA.DAS.EAS.Portal.TypesExtensions;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.Providers.Api.Client;

namespace SFA.DAS.EAS.Portal.Application.EventHandlers.ProviderRelationships
{
    public class AddedAccountProviderEventHandler : IEventHandler<AddedAccountProviderEvent>
    {
        private readonly IAccountDocumentService _accountDocumentService;
        private readonly ILogger<AddedAccountProviderEventHandler> _logger;
        private readonly IProviderApiClient _providerApiClient;

        public AddedAccountProviderEventHandler(
                IAccountDocumentService accountDocumentService,
                ILogger<AddedAccountProviderEventHandler> logger,
                IProviderApiClient providerApiClient)
        {
            _accountDocumentService = accountDocumentService;
            _logger = logger;
            _providerApiClient = providerApiClient;
        }

        public async Task Handle(AddedAccountProviderEvent addedAccountProviderEvent, CancellationToken cancellationToken = default)
        {
            var providerTask = _providerApiClient.GetAsync(addedAccountProviderEvent.ProviderUkprn);
            var accountDocument = await _accountDocumentService.GetOrCreate(addedAccountProviderEvent.AccountId, cancellationToken);
            
            var accountProvider = accountDocument.Account.GetOrAddProvider(addedAccountProviderEvent.ProviderUkprn);

            var provider = await providerTask;
            var address = provider.Addresses.FirstOrDefault(a => a.ContactType == "PRIMARY")
                          ?? provider.Addresses.FirstOrDefault(a => a.ContactType == "LEGAL");
            
            accountProvider.Name = provider.ProviderName;
            accountProvider.Email = provider.Email;
            accountProvider.Phone = provider.Phone;
            accountProvider.Street = address?.Street;
            accountProvider.Town = address?.Town;
            accountProvider.Postcode = address?.PostCode;
                
            await _accountDocumentService.Save(accountDocument, cancellationToken);
        }
    }
}