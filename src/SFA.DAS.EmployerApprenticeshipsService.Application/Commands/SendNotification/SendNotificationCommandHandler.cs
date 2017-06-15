using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Application.Commands.SendNotification
{
    public class SendNotificationCommandHandler : AsyncRequestHandler<SendNotificationCommand>
    {
        private readonly IValidator<SendNotificationCommand> _validator;
        private readonly ILog _logger;
        private readonly INotificationsApi _notificationsApi;

        public SendNotificationCommandHandler(
            IValidator<SendNotificationCommand> validator,
            ILog logger, 
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
                //_logger.Info($"---- === ||| -->     {message.Email.RecipientsAddress}    <-- ||| === ----");
                await _notificationsApi.SendEmail(message.Email);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error sending email to notifications api");
            }
            
        }
    }
}
