using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.Providers.Api.Client;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.Portal.Extensions;
using System.Linq;

namespace SFA.DAS.EAS.Portal.Application.EventHandlers.ProviderRelationships
{
    public class AddedAccountProviderHandler : IEventHandler<AddedAccountProviderEvent>
    {
        private readonly IAccountDocumentService _accountDocumentService;
        private readonly IProviderApiClient _providerApiClient;

        public AddedAccountProviderHandler(
            IAccountDocumentService accountDocumentService,
            IProviderApiClient providerApiClient)
        {
            _accountDocumentService = accountDocumentService;
            _providerApiClient = providerApiClient;
        }

        public async Task Handle(AddedAccountProviderEvent @event, CancellationToken cancellationToken = default)
        {
            var providerTask = _providerApiClient.GetAsync(@event.ProviderUkprn);
            var accountDocument = await _accountDocumentService.GetOrCreate(@event.AccountId, cancellationToken);

            var accountProvider = accountDocument.Account.GetOrAddProvider(@event.ProviderUkprn);

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
