using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Attributes;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging;

namespace SFA.DAS.EAS.Application.Notifications.CreateAgreementSignedMessage
{
    public class CreateAgreementSignedMessageCommandHandler : IAsyncNotificationHandler<CreateAgreementSignedMessageCommand>
    {
        private readonly IMessagePublisher _messagePublisher;
        
        [ServiceBusConnectionKey("tasks")]
        public CreateAgreementSignedMessageCommandHandler(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }
        
        public async Task Handle(CreateAgreementSignedMessageCommand notification)
        {
            await _messagePublisher.PublishAsync(new AgreementSignedMessage
            {
                AccountId = notification.AccountId,
                LegalEntityId = notification.LegalEntityId,
                AgreementId = notification.AgreementId
            });
        }
    }
}
