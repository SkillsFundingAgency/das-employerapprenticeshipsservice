using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Attributes;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging;

namespace SFA.DAS.EAS.Application.Notifications.CreateAgreementCreatedMessage
{
    public class CreateAgreementCreatedMessageCommandHandler : IAsyncNotificationHandler<CreateAgreementCreatedMessageCommand>
    {
        private readonly IMessagePublisher _messagePublisher;

        [QueueName]
        public string agreement_created_notifications { get; set; }

        public CreateAgreementCreatedMessageCommandHandler(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }
     
        public async Task Handle(CreateAgreementCreatedMessageCommand notification)
        {
            await _messagePublisher.PublishAsync(new AgreementCreatedMessage
            {
                AccountId = notification.AccountId,
                LegalEntityId = notification.LegalEntityId,
                AgreementId = notification.AgreementId
            });
        }
    }
}
