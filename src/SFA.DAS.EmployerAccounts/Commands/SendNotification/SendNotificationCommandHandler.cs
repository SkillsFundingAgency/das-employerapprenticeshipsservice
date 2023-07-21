using System.Threading;
using Microsoft.Extensions.Logging;
using SFA.DAS.Notifications.Api.Client;

namespace SFA.DAS.EmployerAccounts.Commands.SendNotification;

public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand>
{
    private readonly IValidator<SendNotificationCommand> _validator;
    private readonly ILogger<SendNotificationCommandHandler> _logger;
    private readonly INotificationsApi _notificationsApi;

    public SendNotificationCommandHandler(
        IValidator<SendNotificationCommand> validator,
        ILogger<SendNotificationCommandHandler> logger,
        INotificationsApi notificationsApi)
    {
        _validator = validator;
        _logger = logger;
        _notificationsApi = notificationsApi;
    }

    public async Task<Unit> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);

        if (!validationResult.IsValid())
        {
            _logger.LogInformation("SendNotificationCommandHandler Invalid Request");
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }
        try
        {
            await _notificationsApi.SendEmail(request.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to notifications api");
        }

        return Unit.Value;
    }
}