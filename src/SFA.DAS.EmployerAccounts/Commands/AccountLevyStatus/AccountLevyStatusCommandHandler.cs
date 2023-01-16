using System.Threading;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.NLog.Logger;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.Commands.AccountLevyStatus;

public class AccountLevyStatusCommandHandler : IRequestHandler<AccountLevyStatusCommand>
{
    private readonly IEmployerAccountRepository _accountRepositoryObject;
    private readonly ILog _logger;
    private readonly IEventPublisher _eventPublisher;

    public AccountLevyStatusCommandHandler(
        IEmployerAccountRepository accountRepositoryObject,
        ILog logger,
        IEventPublisher eventPublisher)
    {
        _accountRepositoryObject = accountRepositoryObject;
        _logger = logger;
        _eventPublisher = eventPublisher;
    }

    public async Task<Unit> Handle(AccountLevyStatusCommand command, CancellationToken cancellationToken)
    {
        var account = await _accountRepositoryObject.GetAccountById(command.AccountId);

        // 1. Prevent setting status to same status
        // 2. Prevent status being changed from Levy to any other status
        // 3. Prevent status being changed to Unknown
        if ((ApprenticeshipEmployerType) account.ApprenticeshipEmployerType == command.ApprenticeshipEmployerType ||
            (ApprenticeshipEmployerType) account.ApprenticeshipEmployerType == ApprenticeshipEmployerType.Levy ||
            command.ApprenticeshipEmployerType == ApprenticeshipEmployerType.Unknown)
        {
            return default;
        }

        _logger.Info(UpdatedStartedMessage(command));

        try
        {
            await _accountRepositoryObject.SetAccountLevyStatus(command.AccountId, command.ApprenticeshipEmployerType);

            await _eventPublisher.Publish(new ApprenticeshipEmployerTypeChangeEvent
            {
                AccountId = command.AccountId,
                ApprenticeshipEmployerType = command.ApprenticeshipEmployerType
            });

            _logger.Info(UpdateCompleteMessage(command));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, UpdateErrorMessage(command));
        }

        return default;
    }

    private static string UpdatedStartedMessage(AccountLevyStatusCommand updateCommand)
    {
        return $"About to update Account with id: {updateCommand.AccountId} to {updateCommand.ApprenticeshipEmployerType} status.";
    }

    private static string UpdateCompleteMessage(AccountLevyStatusCommand updateCommand)
    {
        return $"Updated Account with id: {updateCommand.AccountId} to {updateCommand.ApprenticeshipEmployerType} status.";
    }

    private static string UpdateErrorMessage(AccountLevyStatusCommand updateCommand)
    {
        return $"Error updating Account with id: {updateCommand.AccountId} to {updateCommand.ApprenticeshipEmployerType} status.";
    }
}