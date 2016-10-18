using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.SendNotification
{
    public class SendNotificationCommandHandler : AsyncRequestHandler<SendNotificationCommand>
    {
        private readonly IValidator<SendNotificationCommand> _validator;
        private readonly ILogger _logger;
        private readonly INotificationsApi _notificationsApi;

        public SendNotificationCommandHandler(IValidator<SendNotificationCommand> validator, ILogger logger, INotificationsApi notificationsApi)
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
            
            await _notificationsApi.SendEmail(message.Email);
            
        }
    }
}
