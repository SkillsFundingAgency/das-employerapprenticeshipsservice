using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Client.Database.Models;
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

            if (accountDocument == null)
            {
                accountDocument = AccountDocument.Create(updatedPermissionsEvent.AccountId);
            }
            else
            {
            }
        }
    }
}