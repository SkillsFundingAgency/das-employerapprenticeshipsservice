using System.Threading;
using SFA.DAS.EmployerAccounts.Data.Contracts;

namespace SFA.DAS.EmployerAccounts.Commands.UnsubscribeNotification;

public class UnsubscribeNotificationHandler : IRequestHandler<UnsubscribeNotificationCommand>
{
    private readonly IValidator<UnsubscribeNotificationCommand> _validator;
    private readonly IAccountRepository _accountRepository;

    public UnsubscribeNotificationHandler(
        IValidator<UnsubscribeNotificationCommand> validator,
        IAccountRepository accountRepository)
    {
        _validator = validator;
        _accountRepository = accountRepository;
    }

    public async Task<Unit> Handle(UnsubscribeNotificationCommand command, CancellationToken cancellationToken)
    {
        _validator.Validate(command);

        var settings = await _accountRepository.GetUserAccountSettings(command.UserRef);
        var setting = settings.SingleOrDefault(m => m.AccountId == command.AccountId);

        if (setting == null)
        {
            throw new UnsubscribeNotificationException($"Missing settings for account {command.AccountId} and user with ref {command.UserRef}");
        }

        if (!setting.ReceiveNotifications)
        {
            throw new UnsubscribeNotificationException($"Trying to unsubscribe from an already unsubscribed account, {command.AccountId}");
        }

        setting.ReceiveNotifications = false;
        await _accountRepository.UpdateUserAccountSettings(command.UserRef, settings);

        return Unit.Value;
    }
}