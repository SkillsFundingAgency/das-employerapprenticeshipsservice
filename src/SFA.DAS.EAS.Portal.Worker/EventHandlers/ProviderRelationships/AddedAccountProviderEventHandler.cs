using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EAS.Portal.Application.Commands;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.ProviderRelationships.Messages.Events;

namespace SFA.DAS.EAS.Portal.Worker.EventHandlers.ProviderRelationships
{
    /// <remarks>
    /// Options for retrieving provider details
    /// a) fetch each individual provider when processing each event [gone for this option]
    /// b) bulk fetch all new overnight and keep in local store
    ///
    /// a) will need to update provider details on provider details changed event
    ///    +ve no web job, less storage
    ///    -ve event handling will fail if provider api down (or we don't store provider details)
    /// b) +ve able to handle message even if provider api down, webjob could update stale details
    ///    -ve more data in store, more infrastructure/complexity
    /// </remarks>
    public class AddedAccountProviderEventHandler : IHandleMessages<AddedAccountProviderEvent>
    {
        //todo: if we have time this sprint, subscribe to provider updated event and update our local store provider details
        // if not, add tech-debt item to backlog

        private readonly IPortalCommand<AddedAccountProviderEvent> _addAccountProviderCommand;
        private readonly IMessageContext _messageContext;

        public AddedAccountProviderEventHandler(IPortalCommand<AddedAccountProviderEvent> addAccountProviderCommand, IMessageContext messageContext)
        {
            _addAccountProviderCommand = addAccountProviderCommand;
            _messageContext = messageContext;
        }
        
        public Task Handle(AddedAccountProviderEvent message, IMessageHandlerContext context)
        {
            _messageContext.Initialise(context);
            return _addAccountProviderCommand.Execute(message);
        }
    }
}