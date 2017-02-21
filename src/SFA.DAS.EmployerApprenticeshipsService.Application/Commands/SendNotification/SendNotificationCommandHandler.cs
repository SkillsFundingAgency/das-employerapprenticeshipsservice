using System;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.Notifications.Api.Client;

namespace SFA.DAS.EAS.Application.Commands.SendNotification
{
    public class SendNotificationCommandHandler : AsyncRequestHandler<SendNotificationCommand>
    {
        private readonly IValidator<SendNotificationCommand> _validator;
        private readonly ILogger _logger;
        private readonly INotificationsApi _notificationsApi;

        public SendNotificationCommandHandler(
            IValidator<SendNotificationCommand> validator, 
            ILogger logger, 
            INotificationsApi notificationsApi)
        {
            _validator = validator;
            _logger = logger;
            _notificationsApi = notificationsApi;
        }

        protected override async Task HandleCore(SendNotificationCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                _logger.Info("SendNotificationCommandHandler Invalid Request");
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }
            try
            {
                await _notificationsApi.SendEmail(message.Email);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            
        }
    }
}
