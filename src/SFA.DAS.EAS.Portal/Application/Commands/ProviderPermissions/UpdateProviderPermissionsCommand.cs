using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.ProviderRelationships.Messages.Events;

namespace SFA.DAS.EAS.Portal.Application.Commands.ProviderPermissions
{
    public class UpdateProviderPermissionsCommand
    {
        private readonly IAccountDocumentService _accountDocumentService;
        private readonly ILogger<UpdateProviderPermissionsCommand> _logger;

        public UpdateProviderPermissionsCommand(
            IAccountDocumentService accountDocumentService,
            ILogger<UpdateProviderPermissionsCommand> logger)
        {
            _accountDocumentService = accountDocumentService;
            _logger = logger;
        }

        public async Task Execute(UpdatedPermissionsEvent updatedPermissionsEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Executing UpdateProviderPermissionsCommand");
            
            //todo: move getorcreate/ensure into helper. where? service, base, elsewhere?
            var accountDocument = await _accountDocumentService.Get(updatedPermissionsEvent.AccountId, cancellationToken);
            Organisation organisation;
            
            if (accountDocument == null)
            {
                accountDocument = AccountDocument.Create(updatedPermissionsEvent.AccountId);
                organisation = new Organisation {AccountLegalEntityId = updatedPermissionsEvent.AccountLegalEntityId};
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
                }
            }
            
            organisation.Providers.Add(new Provider
            {
                Ukprn = updatedPermissionsEvent.Ukprn,
                Name = "todo: fetch from fat/new provider service. fetch each message process or keep local store?",
                GrantedOperations = updatedPermissionsEvent.GrantedOperations
            });
        }
    }
}