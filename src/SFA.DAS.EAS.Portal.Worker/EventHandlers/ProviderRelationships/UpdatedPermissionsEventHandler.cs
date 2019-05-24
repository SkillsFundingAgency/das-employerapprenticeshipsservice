using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EAS.Portal.Application.Commands.ProviderPermissions;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Worker.Extensions;
using SFA.DAS.ProviderRelationships.Messages.Events;

namespace SFA.DAS.EAS.Portal.Worker.EventHandlers.ProviderRelationships
{
    public class UpdatedPermissionsEventHandler : IHandleMessages<UpdatedPermissionsEvent>
    {
        private readonly UpdateProviderPermissionsCommand _updateProviderPermissionsCommand;
        private readonly IMessageContext _messageContext;

        public UpdatedPermissionsEventHandler(UpdateProviderPermissionsCommand updateProviderPermissionsCommand, IMessageContext messageContext)
        {
            _updateProviderPermissionsCommand = updateProviderPermissionsCommand;
            _messageContext = messageContext;
        }
        
        public Task Handle(UpdatedPermissionsEvent message, IMessageHandlerContext context)
        {
            _messageContext.Initialise(context);
            return _updateProviderPermissionsCommand.Execute(message);
        }
    }
}