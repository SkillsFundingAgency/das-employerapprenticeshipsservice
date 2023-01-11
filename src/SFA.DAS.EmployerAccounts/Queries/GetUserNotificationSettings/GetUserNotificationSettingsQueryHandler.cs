using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetUserNotificationSettings;

public class GetUserNotificationSettingsQueryHandler: IAsyncRequestHandler<GetUserNotificationSettingsQuery,GetUserNotificationSettingsQueryResponse>
{
    private readonly IValidator<GetUserNotificationSettingsQuery> _validator;
    private readonly IAccountRepository _accountRepository;

    public GetUserNotificationSettingsQueryHandler(IAccountRepository accountRepository, IValidator<GetUserNotificationSettingsQuery> validator)
    {
        _accountRepository = accountRepository;
        _validator = validator;
    }

    public async Task<GetUserNotificationSettingsQueryResponse> Handle(GetUserNotificationSettingsQuery message)
    {
        var validationResult = _validator.Validate(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        var data = await _accountRepository.GetUserAccountSettings(message.UserRef);

        return new GetUserNotificationSettingsQueryResponse
        {
            NotificationSettings = data
        };

    }
}