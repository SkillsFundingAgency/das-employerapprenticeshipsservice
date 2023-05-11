using SFA.DAS.EmployerAccounts.Commands.UnsubscribeNotification;
using SFA.DAS.EmployerAccounts.Commands.UpdateUserNotificationSettings;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;
using SFA.DAS.EmployerAccounts.Queries.GetUserNotificationSettings;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators;

public class UserSettingsOrchestrator
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserSettingsOrchestrator> _logger;
    private readonly IEncodingService _encodingService;
    private readonly EmployerAccountsConfiguration _configuration;

    //Needed for tests
    protected UserSettingsOrchestrator() { }

    public UserSettingsOrchestrator(IMediator mediator, ILogger<UserSettingsOrchestrator> logger, IEncodingService encodingService, EmployerAccountsConfiguration configuration)
    {
        _mediator = mediator;
        _logger = logger;
        _encodingService = encodingService;
        _configuration = configuration;
    }

    public virtual async Task<OrchestratorResponse<NotificationSettingsViewModel>> GetNotificationSettingsViewModel(string userRef)
    {
        _logger.LogInformation("Getting user notification settings for user {UserRef}", userRef);

        var response = await _mediator.Send(new GetUserNotificationSettingsQuery
        {
            UserRef = userRef
        });

        return new OrchestratorResponse<NotificationSettingsViewModel>
        {
            Data = new NotificationSettingsViewModel
            {
                HashedId = userRef,
                NotificationSettings = response.NotificationSettings,
                UseGovSignIn = _configuration.UseGovSignIn
            },
        };
    }

    public virtual async Task UpdateNotificationSettings(string userRef, List<UserNotificationSetting> settings)
    {
        _logger.LogInformation("Updating user notification settings for user {UserRef}", userRef);

        settings.ForEach(s =>
        {
            var accountId = _encodingService.Decode(s.HashedAccountId, EncodingType.AccountId);
            s.AccountId = accountId;
        });

        await _mediator.Send(new UpdateUserNotificationSettingsCommand
        {
            UserRef = userRef,
            Settings = settings
        });
    }

    public async Task<OrchestratorResponse<SummaryUnsubscribeViewModel>> Unsubscribe(
        string userRef,
        string hashedAccountId)
    {
        var accountId = _encodingService.Decode(hashedAccountId, EncodingType.AccountId);

        return await CheckUserAuthorization(
            async () =>
            {
                var settings = await _mediator.Send(new GetUserNotificationSettingsQuery
                {
                    UserRef = userRef
                });

                var userNotificationSettings = settings.NotificationSettings.SingleOrDefault(m => m.AccountId == accountId);

                if (userNotificationSettings == null)
                    throw new InvalidStateException($"Cannot find user settings for user {userRef} in account {accountId}.");

                if (userNotificationSettings.ReceiveNotifications)
                {
                    await _mediator.Send(new UnsubscribeNotificationCommand
                    {
                        UserRef = userRef,
                        AccountId = accountId
                    });

                    _logger.LogInformation("Unsubscribed from alerts for user {UserRef} in account {AccountId}", userRef, accountId);
                }
                else
                {

                    _logger.LogInformation("Already unsubscribed from alerts for user {UserRef} in account {AccountId}", userRef, accountId);
                }

                return new OrchestratorResponse<SummaryUnsubscribeViewModel>
                {
                    Data = new SummaryUnsubscribeViewModel
                    {
                        AlreadyUnsubscribed = !userNotificationSettings.ReceiveNotifications,
                        AccountName = userNotificationSettings.Name
                    }
                };
            }, accountId, userRef);
    }

    protected async Task<OrchestratorResponse<T>> CheckUserAuthorization<T>(Func<Task<OrchestratorResponse<T>>> code, long accountId, string externalUserId) where T : class
    {
        try
        {
            await _mediator.Send(new GetEmployerAccountByIdQuery
            {
                AccountId = accountId,
                UserId = externalUserId
            });

            return await code.Invoke();
        }
        catch (UnauthorizedAccessException exception)
        {
            _logger.LogWarning("User not associated to account. UserId:{ExternalUserId} AccountId:{AccountId}", externalUserId, accountId);

            return new OrchestratorResponse<T>
            {
                Status = HttpStatusCode.Unauthorized,
                Exception = exception
            };
        }
    }
}