using System.Threading;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Commands.SendNotification;

public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand>
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

    public async Task<Unit> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid())
        {
            _logger.Info("SendNotificationCommandHandler Invalid Request");
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }
        try
        {
            await _notificationsApi.SendEmail(request.Email);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error sending email to notifications api");
        }

        return default;
    }
}