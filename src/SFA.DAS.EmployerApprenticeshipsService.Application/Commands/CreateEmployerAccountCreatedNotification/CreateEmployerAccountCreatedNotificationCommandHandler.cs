using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Domain.Attributes;
using SFA.DAS.Messaging;

namespace SFA.DAS.EAS.Application.Commands.CreateEmployerAccountCreatedNotification
{
    public class CreateEmployerAccountCreatedNotificationCommandHandler : AsyncRequestHandler<CreateEmployerAccountCreatedNotificationCommand>
    {
        private readonly IMessagePublisher _messagePublisher;

        [QueueName]
        public string account_created_report_queue { get; set; }

        public CreateEmployerAccountCreatedNotificationCommandHandler(IMessagePublisher messagePublisher)
        {
           _messagePublisher = messagePublisher;
        }

        protected override async Task HandleCore(CreateEmployerAccountCreatedNotificationCommand message)
        {
            await _messagePublisher.PublishAsync(new EmployerAccountCreatedQueueMessage
            {
                AccountId = message.AccountId
            });
        }
    }
}
