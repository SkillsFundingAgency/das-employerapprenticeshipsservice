using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Notification;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.ProcessNotification
{
    public class ProcessNotificationCommandHandler : AsyncRequestHandler<ProcessNotificationCommand>
    {
        private readonly IValidator<ProcessNotificationCommand> _validator;
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger _logger;
        private readonly IEmailService _emailService;


        public ProcessNotificationCommandHandler(IValidator<ProcessNotificationCommand> validator, INotificationRepository notificationRepository, ILogger logger, IEmailService emailService)
        {
            _validator = validator;
            _notificationRepository = notificationRepository;
            _logger = logger;
            _emailService = emailService;
        }

        protected override async Task HandleCore(ProcessNotificationCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                _logger.Info("Invalid Request for ProcessNotificationCommand");
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var notificationMessage = await _notificationRepository.GetById(message.Id);

            if (notificationMessage == null)
            {
                _logger.Info($"Notification record not found in repository for id:{message.Id}");
                throw new InvalidRequestException(new Dictionary<string, string> { { nameof(ProcessNotificationCommand),"Notification record not found in repository for id:{message.Id}"}});
            }

            var content = JsonConvert.DeserializeObject<EmailContent>(notificationMessage.Data);

            await _emailService.SendEmail(new EmailMessage
            {
                Data = content.Data,
                MessageType = notificationMessage.MessageFormat.ToString(),
                ReplyToAddress = content.ReplyToAddress,
                UserId = notificationMessage.UserId,
                RecipientsAddress = content.RecipientsAddress,
                TemplateId = notificationMessage.TemplatedId
            });
        }
    }
}