using SFA.DAS.Audit.Types;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.Validation;
using Entity = SFA.DAS.Audit.Types.Entity;

namespace SFA.DAS.EmployerAccounts.Commands.UpdateUserNotificationSettings;

public class UpdateUserNotificationSettingsCommandHandler :
    AsyncRequestHandler<UpdateUserNotificationSettingsCommand>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IValidator<UpdateUserNotificationSettingsCommand> _validator;
    private readonly IMediator _mediator;

    public UpdateUserNotificationSettingsCommandHandler(IAccountRepository accountRepository,
        IValidator<UpdateUserNotificationSettingsCommand> validator, IMediator mediator)
    {
        _accountRepository = accountRepository;
        _validator = validator;
        _mediator = mediator;
    }

    protected override async Task HandleCore(UpdateUserNotificationSettingsCommand message)
    {
        var validationResult = _validator.Validate(message);

        if (!validationResult.IsValid())
            throw new InvalidRequestException(validationResult.ValidationDictionary);

        await _accountRepository.UpdateUserAccountSettings(message.UserRef, message.Settings);

        foreach (var setting in message.Settings)
        {
            await AddAuditEntry(setting);
        }
    }

    private async Task AddAuditEntry(UserNotificationSetting setting)
    {
        await _mediator.SendAsync(new CreateAuditCommand
        {
            EasAuditMessage = new EasAuditMessage
            {
                Category = "UPDATED",
                Description =
                    $"User {setting.UserId} has updated email notification setting for account {setting.HashedAccountId}",
                ChangedProperties = new List<PropertyUpdate>
                {
                    new PropertyUpdate
                    {
                        PropertyName = "ReceiveNotifications",
                        NewValue = setting.ReceiveNotifications.ToString()
                    }
                },
                RelatedEntities = new List<Entity> {new Entity {Id = setting.UserId.ToString(), Type = "User"}},
                AffectedEntity = new Entity {Type = "UserAccountSetting", Id = setting.Id.ToString()}
            }
        });
    }
}