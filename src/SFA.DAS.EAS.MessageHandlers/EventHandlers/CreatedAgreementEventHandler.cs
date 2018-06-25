using NServiceBus;
using SFA.DAS.EAS.Messages.Events;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.MessageHandlers.EventHandlers
{
    public class CreatedAgreementEventHandler : IHandleMessages<ICreatedAgreementEvent>
    {
        private readonly IMessagePublisher _messagePublisher;

        public CreatedAgreementEventHandler(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }
        public async Task Handle(ICreatedAgreementEvent message, IMessageHandlerContext context)
        {
            await _messagePublisher.PublishAsync(new AgreementCreatedMessage(
                message.AccountId,
                message.AgreementId,
                message.OrganisationName,
                message.LegalEntityId,
                message.UserName,
                message.UserRef.ToString()));
        }
    }
}
