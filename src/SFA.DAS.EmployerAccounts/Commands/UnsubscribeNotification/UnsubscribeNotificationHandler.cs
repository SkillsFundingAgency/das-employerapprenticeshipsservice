using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Commands.UnsubscribeNotification;

public class UnsubscribeNotificationHandler : AsyncRequestHandler<UnsubscribeNotificationCommand>
{
    private readonly IValidator<UnsubscribeNotificationCommand> _validator;
    private readonly INotificationsApi _notificationsApi;
    private readonly IUserRepository _userRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ILog _logger;

    public UnsubscribeNotificationHandler(
        IValidator<UnsubscribeNotificationCommand> validator,
        INotificationsApi notificationsApi,
        IUserRepository userRepository,
        IAccountRepository accountRepository,
        ILog logger)
    {
        _validator = validator;
        _notificationsApi = notificationsApi;
        _userRepository = userRepository;
        _accountRepository = accountRepository;
        _logger = logger;
    }

    protected override async Task HandleCore(UnsubscribeNotificationCommand command)
    {
        _validator.Validate(command);
            
        var settings = await _accountRepository.GetUserAccountSettings(command.UserRef);
        var setting = settings.SingleOrDefault(m => m.AccountId == command.AccountId);
        if (setting == null)
            throw new Exception($"Missing settings for account {command.AccountId} and user with ref {command.UserRef}");
        if(!setting.ReceiveNotifications)
            throw new Exception($"Trying to unsubscribe from an already unsubscribed account, {command.AccountId}");

        setting.ReceiveNotifications = false;
        await _accountRepository.UpdateUserAccountSettings(command.UserRef, settings);
    }
}