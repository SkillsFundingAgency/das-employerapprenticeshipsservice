﻿using System.Threading;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetUserNotificationSettings;

public class GetUserNotificationSettingsQueryHandler: IRequestHandler<GetUserNotificationSettingsQuery,GetUserNotificationSettingsQueryResponse>
{
    private readonly IValidator<GetUserNotificationSettingsQuery> _validator;
    private readonly IAccountRepository _accountRepository;

    public GetUserNotificationSettingsQueryHandler(IAccountRepository accountRepository, IValidator<GetUserNotificationSettingsQuery> validator)
    {
        _accountRepository = accountRepository;
        _validator = validator;
    }

    public async Task<GetUserNotificationSettingsQueryResponse> Handle(GetUserNotificationSettingsQuery message, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(message);

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