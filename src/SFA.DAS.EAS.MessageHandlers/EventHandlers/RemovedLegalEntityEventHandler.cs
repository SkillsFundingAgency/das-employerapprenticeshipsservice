using NServiceBus;
using SFA.DAS.EAS.Messages.Events;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.MessageHandlers.EventHandlers
{
    public class RemovedLegalEntityEventHandler : IHandleMessages<RemovedLegalEntityEvent>
    {
        private readonly IMessagePublisher _messagePublisher;

        public RemovedLegalEntityEventHandler(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public async Task Handle(RemovedLegalEntityEvent message, IMessageHandlerContext context)
        {
            await _messagePublisher.PublishAsync(
                new LegalEntityRemovedMessage(
                    message.AccountId,
                    message.AgreementId,
                    message.AgreementSigned,
                    message.LegalEntityId,
                    message.OrganisationName,
                    message.UserName,
                    message.UserRef.ToString()));
        }
    }
}
