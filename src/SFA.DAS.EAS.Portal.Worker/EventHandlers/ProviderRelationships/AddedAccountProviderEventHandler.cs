using SFA.DAS.EAS.Portal.Application.Commands;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.ProviderRelationships.Messages.Events;

namespace SFA.DAS.EAS.Portal.Worker.EventHandlers.ProviderRelationships
{
    public class AddedAccountProviderEventHandler : EventHandler<AddedAccountProviderEvent>
    {
        public AddedAccountProviderEventHandler(ICommand<AddedAccountProviderEvent> addAccountProviderCommand, IMessageContext messageContext)
            : base(addAccountProviderCommand, messageContext)
        {
        }
    }
}