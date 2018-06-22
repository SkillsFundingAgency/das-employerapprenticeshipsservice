using NServiceBus;
using SFA.DAS.EAS.Messages.Events;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.MessageHandlers.EventHandlers
{
    public class DeletedPayeSchemeEventHandler : IHandleMessages<DeletedPayeSchemeEvent>
    {
        private readonly IMessagePublisher _messagePublisher;

        public DeletedPayeSchemeEventHandler(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public Task Handle(DeletedPayeSchemeEvent message, IMessageHandlerContext context)
        {
            return _messagePublisher.PublishAsync(
                new PayeSchemeDeletedMessage(
                    message.PayeRef,
                    message.OrganisationName,
                    message.AccountId,
                    message.UserName,
                    message.UserRef.ToString()));
        }
    }
}
