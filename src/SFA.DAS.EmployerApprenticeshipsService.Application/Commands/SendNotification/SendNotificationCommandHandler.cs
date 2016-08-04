using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Attributes;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Notification;
using SFA.DAS.Messaging;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.SendNotification
{
    public class SendNotificationCommandHandler : AsyncRequestHandler<SendNotificationCommand>
    {

        [QueueName]
        public string send_notification { get; set; }

        private readonly IValidator<SendNotificationCommand> _validator;
        private readonly ILogger _logger;
        private readonly IMessagePublisher _messagePublisher;
        private readonly INotificationRepository _notificationRepository;

        public SendNotificationCommandHandler(IValidator<SendNotificationCommand> validator, ILogger logger, IMessagePublisher messagePublisher, INotificationRepository notificationRepository)
        {
            _validator = validator;
            _logger = logger;
            _messagePublisher = messagePublisher;
            _notificationRepository = notificationRepository;
        }

        protected override async Task HandleCore(SendNotificationCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                _logger.Info("SendNotificationCommandHandler Invalid Request");
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var messageId = await _notificationRepository.Create(new NotificationMessage
            {
                Data = JsonConvert.SerializeObject(message.Data),
                MessageFormat = message.MessageFormat,
                UserId = message.UserId,
                DateTime = message.DateTime,
                ForceFormat = message.ForceFormat,
                TemplatedId = message.TemplatedId
            });

            _logger.Info($"Notification repository Message: {messageId} created in repository");

            await _messagePublisher.PublishAsync(new SendNotificationQueueMessage {Id = messageId});

            _logger.Info($"SendNotificationQueueMessage Message: {messageId} added to queue");
        }
    }
}
