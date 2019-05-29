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
    public class UpdateProviderPermissionsCommand
    {
        private readonly IAccountDocumentService _accountDocumentService;
        private readonly IProviderApiClient _providerApiClient;
        private readonly ILogger<UpdateProviderPermissionsCommand> _logger;

        public UpdateProviderPermissionsCommand(
            IAccountDocumentService accountDocumentService,
            IProviderApiClient providerApiClient,
            ILogger<UpdateProviderPermissionsCommand> logger)
        {
            _accountDocumentService = accountDocumentService;
            _providerApiClient = providerApiClient;
            _logger = logger;
        }

        public async Task Execute(UpdatedPermissionsEvent updatedPermissionsEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Executing UpdateProviderPermissionsCommand");

            var providerTask = _providerApiClient.GetAsync(updatedPermissionsEvent.Ukprn);
            //todo: move getorcreate/ensure into helper. where? service, base, elsewhere?
            var accountDocument = await _accountDocumentService.Get(updatedPermissionsEvent.AccountId, cancellationToken);
            
            Organisation organisation;
            
            if (accountDocument == null)
            {
                accountDocument = AccountDocument.Create(updatedPermissionsEvent.AccountId);
                //todo: common code
                organisation = new Organisation {AccountLegalEntityId = updatedPermissionsEvent.AccountLegalEntityId};
                accountDocument.Account.Organisations.Add(organisation);
            }
            else
            {
                //todo: this is common code. probably belongs in account, but now we don't have separate read/write models
                // could have account extensions, but then pain to unit test
                // base command?
                organisation = accountDocument.Account.Organisations.FirstOrDefault(o => o.AccountLegalEntityId.Equals(updatedPermissionsEvent.AccountLegalEntityId));
                if (organisation == null)
                {
                    organisation = new Organisation {AccountLegalEntityId = updatedPermissionsEvent.AccountLegalEntityId};
                    accountDocument.Account.Organisations.Add(organisation);
                }
            }

            var provider = await providerTask;
            //todo: log useful message if provider / name not found
            organisation.Providers.Add(new Provider
            {
                Ukprn = updatedPermissionsEvent.Ukprn,
                Name = provider.ProviderName,
                GrantedOperations = updatedPermissionsEvent.GrantedOperations
            });
        }
    }
}