using SFA.DAS.EmployerAccounts.Commands.UnsubscribeNotification;
using SFA.DAS.EmployerAccounts.Commands.UpdateUserNotificationSettings;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;
using SFA.DAS.EmployerAccounts.Queries.GetUserNotificationSettings;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators;

public class UserSettingsOrchestrator
{
    private readonly IMediator _mediator;
    private readonly IHashingService _hashingService;
    private readonly ILogger<UserSettingsOrchestrator> _logger;

    //Needed for tests
    protected UserSettingsOrchestrator() { }

    public UserSettingsOrchestrator(IMediator mediator, IHashingService hashingService, ILogger<UserSettingsOrchestrator> logger)
    {
        _mediator = mediator;
        _hashingService = hashingService;
        _logger = logger;
    }

    public virtual async Task<OrchestratorResponse<NotificationSettingsViewModel>> GetNotificationSettingsViewModel(string userRef)
    {
        _logger.LogInformation($"Getting user notification settings for user {userRef}");

        var response = await _mediator.Send(new GetUserNotificationSettingsQuery
        {
            UserRef = userRef
        });

        return new OrchestratorResponse<NotificationSettingsViewModel>
        {
            Data = new NotificationSettingsViewModel
            {
                HashedId = userRef,
                NotificationSettings = response.NotificationSettings
            },
        };
    }

    public virtual async Task UpdateNotificationSettings(
        string userRef, List<UserNotificationSetting> settings)
    {
        _logger.LogInformation($"Updating user notification settings for user {userRef}");

        DecodeAccountIds(settings);

        await _mediator.Send(new UpdateUserNotificationSettingsCommand
        {
            UserRef = userRef,
            Settings = settings
        });
    }

    public async Task<OrchestratorResponse<SummaryUnsubscribeViewModel>> Unsubscribe(
        string userRef, 
        string hashedAccountId, 
        string settingUrl)
    {
        return await CheckUserAuthorization(
            async () =>
            {
                var accountId = _hashingService.DecodeValue(hashedAccountId);
                var settings = await _mediator.Send(new GetUserNotificationSettingsQuery
                {
                    UserRef = userRef
                });
                        
                var userNotificationSettings = settings.NotificationSettings.SingleOrDefault(m => m.AccountId == accountId);

                if (userNotificationSettings == null)
                    throw new InvalidStateException($"Cannot find user settings for user {userRef} in account {accountId}");

                if (userNotificationSettings.ReceiveNotifications)
                {
                    await _mediator.Send(
                        new UnsubscribeNotificationCommand
                        {
                            UserRef = userRef,
                            AccountId = accountId
                        });

                    _logger.LogInformation("Unsubscribed from alerts for user {userRef} in account {accountId}");
                }
                else {

                    _logger.LogInformation("Already unsubscribed from alerts for user {userRef} in account {accountId}");
                }

                return new OrchestratorResponse<SummaryUnsubscribeViewModel>
                {
                    Data = new SummaryUnsubscribeViewModel
                    {
                        AlreadyUnsubscribed = !userNotificationSettings.ReceiveNotifications,
                        AccountName = userNotificationSettings.Name
                    }
                };
            }, hashedAccountId, userRef);
    }

    private void DecodeAccountIds(List<UserNotificationSetting> source)
    {
        foreach (var setting in source)
        {
            setting.AccountId = _hashingService.DecodeValue(setting.HashedAccountId);
        }
    }

    protected async Task<OrchestratorResponse<T>> CheckUserAuthorization<T>(Func<Task<OrchestratorResponse<T>>> code, string hashedAccountId, string externalUserId) where T : class
    {
        try
        {
            await _mediator.Send(new GetEmployerAccountByHashedIdQuery
            {
                HashedAccountId = hashedAccountId,
                UserId = externalUserId
            });

            return await code.Invoke();
        }
        catch (UnauthorizedAccessException exception)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            _logger.LogWarning($"User not associated to account. UserId:{externalUserId} AccountId:{accountId}");

            return new OrchestratorResponse<T>
            {
                Status = HttpStatusCode.Unauthorized,
                Exception = exception
            };
        }
    }
}